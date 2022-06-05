using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services.Strategy
{
    public class BasicStrategy : IStrategy
    {

        public string Execute(IQuotable quotable, IFundingInfo fundingInfo, BotUser botUser, string fundingSymbol, string strategyConfigJson)
        {
            throw new NotImplementedException();
        }
    }
}
