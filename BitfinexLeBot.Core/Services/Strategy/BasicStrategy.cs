using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.Strategy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services.Strategy
{
    public class BasicStrategy : IStrategy
    {
        public async Task<StrategyResult> ExecuteAsync(IQuoteSource quoteSource, IFundingOperate fundingOperate, BotUser botUser, string fundingSymbol, string strategyConfigJson)
        {
            StrategyResult result = new StrategyResult()
            {
                UserId = botUser.BotUserId.ToString(),
                StrategyName = this.GetType().Name,
                ResultCode = 0
            };

            BasicStrategyConfig config = JsonConvert.DeserializeObject<BasicStrategyConfig>(strategyConfigJson);

            decimal offeringBalance = 0;

            if (config.UpdateOfferingEveryRun)
            {
                var fundingState = await fundingOperate.GetFundingStateAsync(botUser, fundingSymbol);
                if(fundingState.FundingOffers.Count > 0)
                {
                    offeringBalance = fundingState.TotalOfferingAmount;
                    var cancelSignals = await fundingOperate.CancelAllFundingOffers(botUser, fundingSymbol);
                    result.Sinals.AddRange(cancelSignals);
                }
            }

            var fundingBalance = await fundingOperate.GetFundingBalanceAsync(botUser, fundingSymbol);
            if (fundingBalance != null)
            {
                var book = quoteSource.GetFundingBook($"f{fundingSymbol}");

                decimal totalAvailableBalance = fundingBalance.AvailableBalance + offeringBalance;
                if (totalAvailableBalance > 50)
                    result.Sinals.Add(await newOfferAtFirstAskAsync(fundingOperate, botUser, fundingSymbol, totalAvailableBalance, config, book));
            }
            return result;
        }


        private async Task<BitfinexOffer> newOfferAtFirstAskAsync(IFundingOperate fundingOperate, BotUser botUser, string fundingSymbol, decimal amount, BasicStrategyConfig config, BitfinexFundingBook book)
        {
            BitfinexOffer sinal = new BitfinexOffer();

            if (book.Asks == null) return null;

            var offerAmount = StrategyHelper.GetFloorValue(amount, 5);
            var rate = StrategyHelper.GetFloorValue(book.Asks.ToList()[config.FixedAskIndex].Price, 4);
            int period = rate > config.CrazyRate ? config.CrazyRatePeriod : config.FixedPeriod;

            if (rate > config.MinRate)
            {
                sinal = await fundingOperate.NewOffer(botUser, fundingSymbol, offerAmount, rate, period);
            }
            else
            {
                if (!config.DoNotOfferWhenUnderMinRate)
                {
                    sinal = await fundingOperate.NewOffer(botUser, fundingSymbol, offerAmount, config.MinRate, period);
                }
            }
            return sinal;
        }



    }
}
