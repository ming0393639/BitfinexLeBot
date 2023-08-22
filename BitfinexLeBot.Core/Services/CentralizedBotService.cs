using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.FundingInfo;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services;

/// <summary>
/// All registered userStrategies are executed in a worker(single thread)
/// </summary>
public class CentralizedBotService : IBotService
{
    private readonly ILogger<CentralizedBotService> _logger;

    private readonly List<UserStrategy> _registeredUserStrategyList = new();

    /// <summary>
    /// key: UserId; value: BitfinexClient
    /// </summary>
    private readonly Dictionary<int, BitfinexRestClient> _userClientDictionary = new();

    private readonly IQuoteSource _quoteSource;

    private readonly IStrategyService _strategyService;


    public CentralizedBotService(ILogger<CentralizedBotService> logger, IQuoteSource quoteSource, IStrategyService strategyService)
    {
        _logger = logger;
        _quoteSource = quoteSource;

        _strategyService = strategyService;
        InitializeBot();
    }

    public void InitializeBot()
    {
        // read config and regist strategy
        string configPath = Path.Combine(AppContext.BaseDirectory, "UserStrategyConfig.json");
        string content = File.ReadAllText(configPath);
        var userStrategies= JsonSerializer.Deserialize<List<UserStrategy>>(content);
        if (userStrategies != null)
        {
            foreach (var userStrategy in userStrategies)
            {
                _ = RegisterUserStrategy(userStrategy, userStrategy.StrategyConfigJson);
            }
        }
    }

    public bool RegisterUserStrategy(UserStrategy userStrategy, object? strategyConfigJson)
    {
        userStrategy.StrategyConfigJson = strategyConfigJson;

        if (userStrategy.User == null || userStrategy.User.ApiKey == null || userStrategy.User.Secret == null)
        {
            return false;
        }

        var bitfinexRestClient = new BitfinexRestClient(options =>
        {
            options.ApiCredentials = new ApiCredentials(userStrategy.User.ApiKey, userStrategy.User.Secret);
            options.RequestTimeout = TimeSpan.FromSeconds(60);
        });

        _userClientDictionary.Add(userStrategy.User.BotUserId, bitfinexRestClient);

        //_userClientDictionary.Add(userStrategy.User.BotUserId,
        //    new BitfinexClient(new BitfinexClientOptions
        //    {
        //        ApiCredentials = new ApiCredentials(userStrategy.User.ApiKey, userStrategy.User.Secret),
        //        SpotApiOptions = new RestApiClientOptions
        //        {
        //            BaseAddress = "https://api.bitfinex.com/",
        //            RateLimitingBehaviour = RateLimitingBehaviour.Fail
        //        }
        //    }));

        _registeredUserStrategyList.Add(userStrategy);

        return true;
    }

    public void UnregisterUserStrategy(int userStrategyId)
    {
        var userStrategy = _registeredUserStrategyList.Find(u => u.UserStrategyId == userStrategyId);
        if (userStrategy != null)
        {
            _ = _registeredUserStrategyList.Remove(userStrategy);
            _ = _userClientDictionary.Remove(userStrategy.UserStrategyId);
        }
    }

    public List<UserStrategy> GetRegisteredUserStrategy() => _registeredUserStrategyList;

    public FundingState GetFundingState(BotUser user, string fundinSymbol)
    {
        FundingState fundingState = new FundingState();

        var client = _userClientDictionary[user.BotUserId];

        var fundingProvidedResult = client.GeneralApi.Funding.GetFundingInfoAsync($"f{fundinSymbol}");
        if (!fundingProvidedResult.Result.Success)
        {
            _logger.LogError(fundingProvidedResult.Result.Error?.Message);
            return null;
        }

        BitfinexFundingInfo fundingInfo = fundingProvidedResult.Result.Data;
        fundingState.WeightedAvgProvidedRate = fundingInfo.Data.YieldLend * 365;
        fundingState.WeightedAvgProvidedDuration = fundingInfo.Data.DurationLend;

        var fundingCreditsResult = client.GeneralApi.Funding.GetFundingCreditsAsync($"f{fundinSymbol}");
        if (!fundingCreditsResult.Result.Success)
        {
            _logger.LogError(fundingCreditsResult.Result.Error?.Message);
            return null;
        }
        var fundingCreditList = fundingCreditsResult.Result.Data.ToList();
        foreach (var credit in fundingCreditList)
            fundingState.FundingCredits.Add(credit);

        var activeFundingOffersResult = client.GeneralApi.Funding.GetActiveFundingOffersAsync($"f{fundinSymbol}");
        if (!activeFundingOffersResult.Result.Success)
        {
            _logger.LogError(activeFundingOffersResult.Result.Error?.Message);
            return null;
        }
        var fundingOfferList = activeFundingOffersResult.Result.Data.ToList();
        foreach (var offer in fundingOfferList)
            fundingState.FundingOffers.Add(offer);

        return fundingState;
    }

    public FundingBalance GetFundingBalance(BotUser user, string fundinSymbol)
    {
        FundingBalance balance = new FundingBalance();
        var client = _userClientDictionary[user.BotUserId];

        var availableFundingBalanceResult = client.SpotApi.Account.GetAvailableBalanceAsync($"f{fundinSymbol}", OrderSide.Buy, 0, WalletType.Funding);
        if (!availableFundingBalanceResult.Result.Success)
        {
            _logger.LogError(availableFundingBalanceResult.Result.Error?.Message);
            return null;
        }
        balance.AvailableBalance = -availableFundingBalanceResult.Result.Data.AvailableBalance;
        //balance.AvailableBalance = Math.Floor(-availableFundingBalanceResult.Data.AvailableBalance * 1000000) / 1000000;

        var fundingBalanceResult = client.SpotApi.Account.GetBalancesAsync();
        var wallet = fundingBalanceResult.Result.Data
            .Where(b => b.Type.Equals(WalletType.Funding) && b.Asset.Equals(fundinSymbol)).First();
        if (wallet != null)
            balance.TotalBalance = wallet.Total;
        return balance;
    }

    public FundingPerformance GetFundingPerformance(BotUser user, string fundinSymbol)
    {
        FundingPerformance performance = new FundingPerformance();
        var client = _userClientDictionary[user.BotUserId];

        var ledgerEntriesResult = client.SpotApi.Account.GetLedgerEntriesAsync(fundinSymbol, DateTime.Now.AddYears(-5), DateTime.Now, 2500, 28);
        var ledgerEntries = ledgerEntriesResult.Result.Data;
        foreach (var entry in ledgerEntries)
        {
            if (entry.Description.Equals("Margin Funding Payment on wallet funding"))
                performance.Profits.Add(new ProfitInfo(entry.Quantity, entry.Timestamp));
        }
        return performance;
    }

    public BitfinexOffer CancelFundingOffer(BotUser user, long id)
    {
        var client = _userClientDictionary[user.BotUserId];
        var cancelOfferResult = client.GeneralApi.Funding.CancelOfferAsync(id);
        return cancelOfferResult.Result.Data;
    }

    public List<BitfinexOffer> CancelAllFundingOffers(BotUser user, string fundinSymbol)
    {
        List<BitfinexOffer> result = new List<BitfinexOffer>();
        var fundingState = GetFundingState(user, fundinSymbol);
        if (fundingState == null)
            return result;
        foreach (var offer in fundingState.FundingOffers)
            result.Add(CancelFundingOffer(user, offer.Id));
        return result;
    }

    public BitfinexOffer NewOffer(BotUser user, string fundinSymbol, decimal amount, decimal rate, int period = 2)
    {
        var client = _userClientDictionary[user.BotUserId];
        var result = client.GeneralApi.Funding.NewOfferAsync(fundinSymbol, amount, rate, period, FundingType.Lend);
        return result.Result.Data;
    }

    public void RunStep()
    {
        _logger.LogInformation("CentralizedBotService RunStep.");
        foreach (var userStrategy in _registeredUserStrategyList)
        {
            if (!userStrategy.Active)
            {
                continue;
            }
            if (userStrategy.StrategyName != null && userStrategy.User != null
                && userStrategy.FundingSymbol != null && userStrategy.StrategyConfigJson != null)
            {
                var strategy = _strategyService.GetStrategy(userStrategy.StrategyName);
                var strategyResult = strategy?.Execute(
                    _quoteSource, this, userStrategy.User, userStrategy.FundingSymbol, userStrategy.StrategyConfigJson);
                _logger.LogInformation(JsonSerializer.Serialize(strategyResult));
            }
        }
        _logger.LogInformation("CentralizedBotService RunStep End.");
    }
}
