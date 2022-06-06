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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services
{
    public class CentralizedBotService : IBotService
    {

        private List<UserStrategy> registeredUserStrategyList = new List<UserStrategy>();

        /// <summary>
        /// key: UserId; value: BitfinexClient
        /// </summary>
        private Dictionary<int, BitfinexClient> userClientDictionary = new Dictionary<int, BitfinexClient>();

        private BackgroundWorker worker = new BackgroundWorker();

        private IQuoteSource quoteSource;

        private IStrategyService strategyService;


        public CentralizedBotService(IQuoteSource quoteSource, IStrategyService strategyService)
        {
            this.quoteSource = quoteSource;
            this.strategyService = strategyService;
        }


        public void InitializeBot()
        {
            worker.DoWork += new DoWorkEventHandler(botDoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(botRunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(botProgressChanged);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.RunWorkerAsync();
        }

        private void botDoWork(object sender, DoWorkEventArgs e)
        {
            while (!worker.CancellationPending)
            {
                foreach (var userStrategy in registeredUserStrategyList)
                {
                    if(!userStrategy.Active)
                    {
                        continue;
                    }
                    var strategy = strategyService.GetStrategy(userStrategy.StrategyName);
                    if (strategy != null)
                        strategy.ExecuteAsync(quoteSource, this, userStrategy.User, userStrategy.FundingSymbol, userStrategy.StrategyConfigJson);

                }

                Thread.Sleep(100);
            }

        }

        private void botProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void botRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }


        public bool RegisterUserStrategy(UserStrategy userStrategy, string strategyConfigJson)
        {
            userStrategy.StrategyConfigJson = strategyConfigJson;

            userClientDictionary.Add(userStrategy.User.BotUserId, 
                new BitfinexClient(new BitfinexClientOptions
            {
                ApiCredentials = new ApiCredentials(userStrategy.User.ApiKey, userStrategy.User.Secret),
                SpotApiOptions = new RestApiClientOptions
                {
                    BaseAddress = "https://api.bitfinex.com/",
                    RateLimitingBehaviour = RateLimitingBehaviour.Fail
                }
            }));

            return true;
        }

        public void UnregisterUserStrategy(int userStrategyId)
        {
            var userStrategy = registeredUserStrategyList.Find(u => u.UserStrategyId == userStrategyId);
            if (userStrategy != null)
            {
                registeredUserStrategyList.Remove(userStrategy);
                userClientDictionary.Remove(userStrategy.UserStrategyId);
            }
        }

        public List<UserStrategy> GetRegisteredUserStrategy()
        {
            return registeredUserStrategyList;
        }

        public FundingState GetFundingState(BotUser user, string fundinSymbol)
        {
            FundingState fundingState = new FundingState();

            BitfinexClient client = userClientDictionary[user.BotUserId];

            var fundingProvidedResult = client.GeneralApi.Funding.GetFundingInfoAsync($"f{fundinSymbol}");
            BitfinexFundingInfo fundingInfo = fundingProvidedResult.Result.Data;
            fundingState.WeightedAvgProvidedRate = fundingInfo.Data.YieldLend * 365;
            fundingState.WeightedAvgProvidedDuration = fundingInfo.Data.DurationLend;

            var fundingCreditsResult = client.GeneralApi.Funding.GetFundingCreditsAsync($"f{fundinSymbol}");
            var fundingCreditList = fundingCreditsResult.Result.Data.ToList();
            foreach (var credit in fundingCreditList)
                fundingState.FundingCredits.Add(credit);

            var activeFundingOffersResult = client.GeneralApi.Funding.GetActiveFundingOffersAsync($"f{fundinSymbol}");
            var fundingOfferList = activeFundingOffersResult.Result.Data.ToList();
            foreach (var offer in fundingOfferList)
                fundingState.FundingOffers.Add(offer);

            return fundingState;
        }

        public FundingBalance GetFundingBalance(BotUser user, string fundinSymbol)
        {
            FundingBalance balance = new FundingBalance();
            BitfinexClient client = userClientDictionary[user.BotUserId];

            var availableFundingBalanceResult = client.SpotApi.Account.GetAvailableBalanceAsync($"f{fundinSymbol}", OrderSide.Buy, 0, WalletType.Funding);
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
            BitfinexClient client = userClientDictionary[user.BotUserId];

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
            BitfinexClient client = userClientDictionary[user.BotUserId];
            var cancelOfferResult = client.GeneralApi.Funding.CancelOfferAsync(id);
            return cancelOfferResult.Result.Data;
        }

        public List<BitfinexOffer> CancelAllFundingOffers(BotUser user, string fundinSymbol)
        {
            var fundingState = GetFundingState(user, fundinSymbol);
            List<BitfinexOffer> result = new List<BitfinexOffer>();
            foreach (var offer in fundingState.Result.FundingOffers)
                result.Add(CancelFundingOffer(user, offer.Id));
            return result;
        }

        public  BitfinexOffer NewOffer(BotUser user, string fundinSymbol, decimal amount, decimal rate, int period = 2)
        {
            BitfinexClient client = userClientDictionary[user.BotUserId];
            var result = client.GeneralApi.Funding.NewOfferAsync(fundinSymbol, amount, rate, period, FundingType.Lend);
            return result.Result.Data;
        }
    }
}
