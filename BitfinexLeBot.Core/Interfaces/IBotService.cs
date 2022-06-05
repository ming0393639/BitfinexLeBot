using BitfinexLeBot.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Interfaces
{
    public interface IBotService
    {
        BackgroundWorker RunUserStrategy(string botLabel, string apiKey, string apiSecret, string fundingSymbol, string strategyName, string strategyConfigJson);

        void StopUserStrategy(string botLabel);

        Dictionary<string, BackgroundWorker> GetUserStrategyBotDictionary();

    }
}
