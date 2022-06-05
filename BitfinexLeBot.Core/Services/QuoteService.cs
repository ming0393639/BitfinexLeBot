using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services
{
    public class QuoteService : IQuoteService
    {
        BitfinexClient client = new BitfinexClient();
        BitfinexSocketClient socketClient = new BitfinexSocketClient();

        public void InitializeQuote()
        {
            throw new NotImplementedException();
        }

        public BitfinexClient GetBitfinexClient()
        {
            return client;
        }

        public BitfinexSocketClient GetBitfinexSocketClient()
        {
            return socketClient;
        }

        public BitfinexFundingBook GetFundingBook(string currency, int limit = 10)
        {
            throw new NotImplementedException();
        }

        public List<BitfinexKline> GetKLines(string symbol, DateTime startTime, DateTime? endTime = null, KlineInterval timeFrame = KlineInterval.FifteenMinutes)
        {
            throw new NotImplementedException();
        }

        public List<BitfinexKline> GetKLines(string symbol, int amount, KlineInterval timeFrame = KlineInterval.FifteenMinutes)
        {
            throw new NotImplementedException();
        }

        public BitfinexKline GetLastKLine(string symbol, KlineInterval timeFrame = KlineInterval.FifteenMinutes)
        {
            throw new NotImplementedException();
        }

        public List<BitfinexSymbolOverview> GetSymbolsOverview(params string[] symbols)
        {
            throw new NotImplementedException();
        }

        public List<BitfinexTradeSimple> GetTrades(string symbol, int limit = 100)
        {
            throw new NotImplementedException();
        }

    }
}
