using BitfinexLeBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Interfaces
{
    public interface IStrategy
    {

        string Execute(IQuotable quotable, IFundingInfo fundingInfo, BotUser botUser, string fundingSymbol, string strategyConfigJson);


    }
}
