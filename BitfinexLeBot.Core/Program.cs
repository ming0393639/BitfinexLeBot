// See https://aka.ms/new-console-template for more information
using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects;
using BitfinexLeBot.Core.Models.FundingInfo;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;



FundingPerformance performance = new FundingPerformance();

BitfinexClient client = new BitfinexClient(new BitfinexClientOptions
{
    ApiCredentials = new ApiCredentials("atf3viarYpVsThhcAteqV4xILMk8TCXpRLg3nc9TXaP", "bkCBKgWi1Xvskzug4hIVdpRP3oojJ47Ltje8TEOi6xj"),
    SpotApiOptions = new RestApiClientOptions
    {
        BaseAddress = "https://api.bitfinex.com/",
        RateLimitingBehaviour = RateLimitingBehaviour.Fail
    }
});

var ledgerEntriesResult = await client.SpotApi.Account.GetLedgerEntriesAsync("USD", DateTime.Now.AddYears(-5), DateTime.Now, 2500, 28);
var ledgerEntries = ledgerEntriesResult.Data;
int i = 0;
foreach (var entry in ledgerEntries)
{
    if (entry.Description.Equals("Margin Funding Payment on wallet funding"))
        performance.Profits.Add(new ProfitInfo(entry.Quantity, entry.Timestamp));
}



var result = await client.SpotApi.ExchangeData.GetKlinesAsync("fUSD", Bitfinex.Net.Enums.KlineInterval.OneMinute, "p120");
Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

BitfinexSocketClient socketClient = new Bitfinex.Net.Clients.BitfinexSocketClient();
var ret = socketClient.SpotStreams.SubscribeToOrderBookUpdatesAsync("tBTCUSD", Bitfinex.Net.Enums.Precision.PrecisionLevel0, Bitfinex.Net.Enums.Frequency.TwoSeconds, 25, d =>
{
    Console.WriteLine(JsonConvert.SerializeObject(d, Formatting.Indented));
});

Console.WriteLine("Hello, World!");

Console.ReadLine();
