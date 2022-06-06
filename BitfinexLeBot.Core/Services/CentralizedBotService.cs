using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Models;
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
                        strategy.Execute(quoteSource, this, userStrategy.User, userStrategy.FundingSymbol, userStrategy.StrategyConfigJson);

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

        public async Task<FundingState> GetFundingStateAsync(UserStrategy userStrategy)
        {
            FundingState fundingState = new FundingState();

            BitfinexClient client = userClientDictionary[userStrategy.User.BotUserId];

            var fundingProvidedResult = await client.GeneralApi.Funding.GetFundingInfoAsync($"f{userStrategy.FundingSymbol}");
            BitfinexFundingInfo fundingInfo = fundingProvidedResult.Data;
            fundingState.WeightedAvgProvidedRate = fundingInfo.Data.YieldLend * 365;
            fundingState.WeightedAvgProvidedDuration = fundingInfo.Data.DurationLend;

            var fundingCreditsResult = await client.GeneralApi.Funding.GetFundingCreditsAsync($"f{userStrategy.FundingSymbol}");
            var fundingCreditList = fundingCreditsResult.Data.ToList();
            foreach (var credit in fundingCreditList)
                fundingState.FundingCredits.Add(credit);

            var activeFundingOffersResult = await client.GeneralApi.Funding.GetActiveFundingOffersAsync("fUSD");
            var fundingOfferList = activeFundingOffersResult.Data.ToList();
            foreach (var offer in fundingOfferList)
                fundingState.FundingOffers.Add(offer);

            return fundingState;
        }

        public async Task<FundingBalance> GetFundingBalanceAsync(UserStrategy userStrategy)
        {
            FundingBalance balance = new FundingBalance();
            BitfinexClient client = userClientDictionary[userStrategy.User.BotUserId];

            var availableFundingBalanceResult = await client.SpotApi.Account.GetAvailableBalanceAsync($"f{userStrategy.FundingSymbol}", OrderSide.Buy, 0, WalletType.Funding);
            balance.AvailableBalance = -availableFundingBalanceResult.Data.AvailableBalance;
            //balance.AvailableBalance = Math.Floor(-availableFundingBalanceResult.Data.AvailableBalance * 1000000) / 1000000;

            var fundingBalanceResult = await client.SpotApi.Account.GetBalancesAsync();
            var wallet = fundingBalanceResult.Data
                .Where(b => b.Type.Equals(WalletType.Funding) && b.Asset.Equals(userStrategy.FundingSymbol)).First();
            if (wallet != null)
                balance.TotalBalance = wallet.Total;
            return balance;
        }

        public async Task<FundingPerformance> GetFundingPerformanceAsync(UserStrategy userStrategy)
        {
            FundingPerformance performance = new FundingPerformance();
            BitfinexClient client = userClientDictionary[userStrategy.User.BotUserId];

            var ledgerEntriesResult = await client.SpotApi.Account.GetLedgerEntriesAsync(userStrategy.FundingSymbol, DateTime.Now.AddYears(-5), DateTime.Now, 2500, 28);
            var ledgerEntries = ledgerEntriesResult.Data;
            foreach (var entry in ledgerEntries)
            {
                if (entry.Description.Equals("Margin Funding Payment on wallet funding"))
                    performance.Profits.Add(new ProfitInfo(entry.Quantity, entry.Timestamp));
            }
            return performance;
        }
    }
}
