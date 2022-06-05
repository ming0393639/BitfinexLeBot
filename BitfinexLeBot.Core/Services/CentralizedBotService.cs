using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects;
using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
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


        public void InitializeBot()
        {
            throw new NotImplementedException();
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


    }
}
