using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services.Strategy
{
    public class RecentHighStrategy : IStrategy
    {
        public Task<StrategyResult> ExecuteAsync(IQuoteSource quoteSource, IFundingOperate fundingOperate, BotUser botUser, string fundingSymbol, string strategyConfigJson)
        {
            throw new NotImplementedException();
        }
    }
}
