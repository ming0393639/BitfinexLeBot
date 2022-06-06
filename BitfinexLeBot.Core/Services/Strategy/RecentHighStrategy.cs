using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.Strategy;
using Newtonsoft.Json;

namespace BitfinexLeBot.Core.Services.Strategy
{
    public class RecentHighStrategy : IStrategy
    {
        public StrategyResult Execute(IQuoteSource quoteSource, IFundingOperate fundingOperate, BotUser botUser, string fundingSymbol, string strategyConfigJson)
        {
            StrategyResult result = new StrategyResult()
            {
                UserId = botUser.BotUserId.ToString(),
                StrategyName = this.GetType().Name,
                ResultCode = 0
            };

            RecentHighStrategyConfig config = JsonConvert.DeserializeObject<RecentHighStrategyConfig>(strategyConfigJson);

            decimal offeringBalance = 0;

            if (config.UpdateOfferingEveryRun)
            {
                var fundingState = fundingOperate.GetFundingState(botUser, fundingSymbol);
                if (fundingState.FundingOffers.Count > 0)
                {
                    offeringBalance = fundingState.TotalOfferingAmount;
                    var cancelSignals = fundingOperate.CancelAllFundingOffers(botUser, fundingSymbol);
                    result.Sinals.AddRange(cancelSignals);
                }
            }

            // Send orders
            var fundingBalance = fundingOperate.GetFundingBalance(botUser, fundingSymbol);
            if (fundingBalance != null)
            {
                var trades = quoteSource.GetTrades($"f{fundingSymbol}", config.RecentHighTradeCount);
                var kLines = quoteSource.GetKLines($"f{fundingSymbol}", config.RecentHighKCount);

                decimal totalAvailableBalance = fundingBalance.AvailableBalance + offeringBalance;
                if (totalAvailableBalance > 50)
                {
                    var signal = newOfferAtFirstAsk(fundingOperate, botUser, fundingSymbol, totalAvailableBalance, config, fundingBalance.TotalBalance, trades, kLines);
                    if (signal!=null)
                        result.Sinals.Add(signal);
                }
            }
            return result;
        }

        private BitfinexOffer newOfferAtFirstAsk(IFundingOperate fundingOperate, BotUser botUser, string fundingSymbol, decimal amount,
            RecentHighStrategyConfig config, decimal totalBalance, List<BitfinexTradeSimple> trades, List<BitfinexKline> kLines)
        {
            BitfinexOffer sinal = new BitfinexOffer();

            var offerAmount = StrategyHelper.GetFloorValue(amount, 5);

            decimal maxR = 0;
            int tCount = config.RecentHighTradeCount > trades.Count ? trades.Count : config.RecentHighTradeCount;
            var usedTrades = trades.GetRange(0, tCount);
            foreach (var t in usedTrades)
            {
                maxR = maxR > t.Price ? maxR : t.Price;
            }

            int kCount = config.RecentHighKCount > kLines.Count ? kLines.Count : config.RecentHighKCount;
            var usedKLines = kLines.GetRange(0, kCount);
            foreach (var k in usedKLines)
            {
                maxR = maxR > k.HighPrice ? maxR : k.HighPrice;
            }

            decimal rate = maxR * 36500 * config.OrderRateFactor;

            int period = rate > config.CrazyRate ? config.CrazyRatePeriod : config.FixedPeriod;

            if (config.CrazyReservedSharePercent > 0)
            {
                decimal crazyAmount = totalBalance * config.CrazyReservedSharePercent / 100;
                if (rate < config.CrazyRate && crazyAmount >= 50)
                {
                    if (offerAmount > crazyAmount)
                    {
                        crazyAmount = offerAmount - crazyAmount > 50 ? crazyAmount : offerAmount;
                        offerAmount -= crazyAmount;
                    }
                    else
                    {
                        offerAmount = 0;
                    }
                }
            }

            // normal offer
            if (offerAmount > 50)
            {
                if (rate > config.MinRate)
                    sinal = fundingOperate.NewOffer(botUser, fundingSymbol, offerAmount, rate, period);
                else
                {
                    if (!config.DoNotOfferWhenUnderMinRate)
                        sinal = fundingOperate.NewOffer(botUser, fundingSymbol, offerAmount, config.MinRate, period);
                }
            }

            return sinal;
        }

    }
}
