using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Interfaces
{
    public interface IQuoteService
    {
        void InitializeQuote();

        BitfinexClient GetBitfinexClient();

        BitfinexSocketClient GetBitfinexSocketClient();

        BitfinexKline GetLastKLine(string symbol, KlineInterval timeFrame = KlineInterval.FifteenMinutes);

        List<BitfinexKline> GetKLines(string symbol, DateTime startTime, DateTime? endTime = null, KlineInterval timeFrame = KlineInterval.FifteenMinutes);

        List<BitfinexKline> GetKLines(string symbol, int amount, KlineInterval timeFrame = KlineInterval.FifteenMinutes);

        BitfinexFundingBook GetFundingBook(string currency, int limit = 10);

        List<BitfinexTradeSimple> GetTrades(string symbol, int limit = 100);

        List<BitfinexSymbolOverview> GetSymbolsOverview(params string[] symbols);




    }
}
