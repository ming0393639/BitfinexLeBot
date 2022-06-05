using Bitfinex.Net.Clients;

namespace BitfinexLeBot.Core.Interfaces
{
    public interface IQuoteService : IQuotable
    {
        void InitializeQuote(List<string> symbolList);




    }
}
