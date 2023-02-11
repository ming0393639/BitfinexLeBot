using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.FundingInfo;

namespace BitfinexLeBot.Core.Interfaces;

public interface IFundingOperate
{

    FundingState GetFundingState(BotUser user, string fundinSymbol);

    FundingBalance GetFundingBalance(BotUser user, string fundinSymbol);

    FundingPerformance GetFundingPerformance(BotUser user, string fundinSymbol);


    BitfinexOffer CancelFundingOffer(BotUser user, long id);

    List<BitfinexOffer> CancelAllFundingOffers(BotUser user, string fundinSymbol);

    BitfinexOffer NewOffer(BotUser user, string fundinSymbol, decimal amount, decimal rate, int period = 2);

}
