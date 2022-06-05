﻿using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects;
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

        List<UserStrategy> registeredUserStrategyList = new List<UserStrategy>();

        /// <summary>
        /// key: UserId; value: BitfinexClient
        /// </summary>
        Dictionary<int, BitfinexClient> userClientDictionary = new Dictionary<int, BitfinexClient>();

        BackgroundWorker worker = new BackgroundWorker();



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
            }
        }

        public List<UserStrategy> GetRegisteredUserStrategy()
        {
            return registeredUserStrategyList;
        }

        public FundingState GetFundingState(BotUser user)
        {
            throw new NotImplementedException();
        }

        public FundingBalance GetFundingBalance(BotUser user)
        {
            throw new NotImplementedException();
        }

        public FundingPerformance GetFundingPerformance(BotUser user)
        {
            throw new NotImplementedException();
        }
    }
}
