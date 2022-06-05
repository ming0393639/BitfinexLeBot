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

        string Execute(IQuoteSource quoteSource, IFundingInfo fundingInfo, BotUser botUser, string fundingSymbol, string strategyConfigJson);


    }
}
