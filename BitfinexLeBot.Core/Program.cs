// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;

//Bitfinex.Net.Clients.BitfinexClient client = new Bitfinex.Net.Clients.BitfinexClient();
//var result = await client.SpotApi.ExchangeData.GetKlinesAsync("fUSD", Bitfinex.Net.Enums.KlineInterval.OneMinute, "p120");
//Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

Bitfinex.Net.Clients.BitfinexSocketClient socketClient = new Bitfinex.Net.Clients.BitfinexSocketClient();
var ret = socketClient.SpotStreams.SubscribeToOrderBookUpdatesAsync("tBTCUSD", Bitfinex.Net.Enums.Precision.PrecisionLevel0, Bitfinex.Net.Enums.Frequency.TwoSeconds, 25, d =>
{
    Console.WriteLine(JsonConvert.SerializeObject(d, Formatting.Indented));
});

Console.WriteLine("Hello, World!");

Console.ReadLine();
