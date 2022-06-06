using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.FundingInfo;

namespace BitfinexLeBot.Core.Interfaces
{
    public interface IFundingOperate
    {

        Task<FundingState> GetFundingStateAsync(BotUser user, string fundinSymbol);

        Task<FundingBalance> GetFundingBalanceAsync(BotUser user, string fundinSymbol);

        Task<FundingPerformance> GetFundingPerformanceAsync(BotUser user, string fundinSymbol);


        Task<BitfinexOffer> CancelFundingOffer(BotUser user, long id);

        Task<List<BitfinexOffer>> CancelAllFundingOffers(BotUser user, string fundinSymbol);

        Task<BitfinexOffer> NewOffer(BotUser user, string fundinSymbol, decimal amount, decimal rate, int period = 2);

    }
}
