using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services.Quote;

public class NormalQuoteService : QuoteService
{

    public NormalQuoteService()
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, "UserStrategyConfig.json");
        string content = File.ReadAllText(configPath);
        var userStrategies = JsonSerializer.Deserialize<List<UserStrategy>>(content);


        var client = new BitfinexRestClient(options =>
        {
            options.ApiCredentials = new ApiCredentials(userStrategies[0].User.ApiKey, userStrategies[0].User.Secret);
            options.RequestTimeout = TimeSpan.FromSeconds(60);
        });

        //var client = new BitfinexClient(new BitfinexClientOptions
        //{
        //    ApiCredentials = new ApiCredentials(userStrategies[0].User.ApiKey, userStrategies[0].User.Secret),
        //    SpotApiOptions = new RestApiClientOptions
        //    {
        //        BaseAddress = "https://api.bitfinex.com/",
        //        RateLimitingBehaviour = RateLimitingBehaviour.Fail
        //    }
        //});

        _client = client;
        _socketClient = null;
    }

}
