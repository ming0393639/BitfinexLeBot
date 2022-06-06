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
        public StrategyResult Execute(IQuoteSource quoteSource, IFundingInfo fundingInfo, BotUser botUser, string fundingSymbol, string strategyConfigJson)
        {
            StrategyResult result = new StrategyResult();

            BasicStrategyConfig config = JsonConvert.DeserializeObject<BasicStrategyConfig>(strategyConfigJson);

            decimal offeringBalance = 0;

            //if (config.UpdateOfferingEveryRun &&
            //    bot.FundingState != null && bot.FundingState.FundingOffers.Count > 0)
            //{
            //    offeringBalance = bot.FundingState.TotalOfferingAmount;
            //    StrategyResult cancelRes = new StrategyResult() { Type = "CancelFundingOffers" };
            //    cancelRes.Sinals.AddRange(bot.CancelAllFundingOffers());
            //    resultList.Add(cancelRes);

            //    //resultList.Add(NewOfferAtFirstAsk(bot, config, book));
            //}

            //if (bot.FundingBalance != null)
            //{
            //    decimal totalAvailableBalance = bot.FundingBalance.AvailableBalance + offeringBalance;
            //    if (totalAvailableBalance > 50)
            //        resultList.Add(NewOfferAtFirstAsk(bot, totalAvailableBalance, config, book));
            //}
            return result;
        }





    }
}
