using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using CryptoExchange.Net.Sockets;

namespace BitfinexLeBot.Core.Interfaces;

public interface IQuoteSource
{
    //void InitializeQuote(BitfinexClient client, BitfinexSocketClient _socketClient);

    //BitfinexClient GetBitfinexClient();

    //BitfinexSocketClient GetBitfinexSocketClient();

    BitfinexKline GetLastKLine(string symbol, KlineInterval timeFrame = KlineInterval.FifteenMinutes);

    List<BitfinexKline> GetKLines(string symbol, DateTime startTime, DateTime? endTime = null, KlineInterval timeFrame = KlineInterval.FifteenMinutes);

    List<BitfinexKline> GetKLines(string symbol, int amount, KlineInterval timeFrame = KlineInterval.FifteenMinutes);

    BitfinexFundingBook GetFundingBook(string currency, int limit = 10);

    List<BitfinexTradeSimple> GetTrades(string symbol, int limit = 100);

    List<BitfinexSymbolOverview> GetSymbolsOverview(params string[] symbols);

    void SubscribeBookUpdated(Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler);




}
