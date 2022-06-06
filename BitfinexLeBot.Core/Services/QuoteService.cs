using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Interfaces;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services
{
    public class QuoteService : IQuoteSource
    {
        BitfinexClient client = new BitfinexClient();
        BitfinexSocketClient socketClient = new BitfinexSocketClient();



        //public void InitializeQuote(List<string> symbolList)
        //{
        //    throw new NotImplementedException();
        //}

        //public BitfinexClient GetBitfinexClient()
        //{
        //    return client;
        //}

        //public BitfinexSocketClient GetBitfinexSocketClient()
        //{
        //    return socketClient;
        //}

        public BitfinexFundingBook GetFundingBook(string currency, int limit = 25)
        {
            return client.SpotApi.ExchangeData.GetFundingBookAsync(currency, limit).Result.Data;
        }

        public List<BitfinexKline> GetKLines(string symbol, DateTime startTime, DateTime? endTime = null, KlineInterval timeFrame = KlineInterval.FifteenMinutes)
        {
            endTime = endTime == null ? DateTime.Now : endTime;
            var candleResult = client.SpotApi.ExchangeData.GetKlinesAsync(symbol, timeFrame, "p2", 5000, startTime.ToUniversalTime(), endTime?.ToUniversalTime());
            return candleResult.Result.Success ? candleResult.Result.Data.ToList() : null;
        }

        public List<BitfinexKline> GetKLines(string symbol, int amount, KlineInterval timeFrame = KlineInterval.FifteenMinutes)
        {
            if (amount <= 0) return new List<BitfinexKline>();
            var candleResult = client.SpotApi.ExchangeData.GetKlinesAsync(symbol, timeFrame, "p2", amount);
            return candleResult.Result.Success ? candleResult.Result.Data.ToList() : null;
        }

        public BitfinexKline GetLastKLine(string symbol, KlineInterval timeFrame = KlineInterval.FifteenMinutes)
        {
            var candleResult = client.SpotApi.ExchangeData.GetLastKlineAsync(symbol, timeFrame, "p2");
            return candleResult.Result.Data;
        }

        public List<BitfinexSymbolOverview> GetSymbolsOverview(params string[] symbols)
        {
            var result = client.SpotApi.ExchangeData.GetTickersAsync(symbols);
            return result.Result.Success ? result.Result.Data.ToList() : null;
        }

        public List<BitfinexTradeSimple> GetTrades(string symbol, int limit = 100)
        {
            var result = client.SpotApi.ExchangeData.GetTradeHistoryAsync(symbol, limit);
            return result.Result.Success ? result.Result.Data.ToList() : null;
        }

        public void SubscribeBookUpdated(Action<DataEvent<IEnumerable<BitfinexOrderBookEntry>>> handler)
        {
            throw new NotImplementedException();
        }
    }
}
