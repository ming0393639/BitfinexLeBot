using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Services.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services
{
    public class StrategyPoolService : IStrategyService
    {
        readonly Dictionary<string, IStrategy> strategyDictionary = new Dictionary<string, IStrategy>();
        

        public StrategyPoolService()
        {
            strategyDictionary.Add("Basic", new BasicStrategy());
            strategyDictionary.Add("RecentHigh", new RecentHighStrategy());

        }

        public IStrategy GetStrategy(string strategyName)
        {
            return strategyDictionary.ContainsKey(strategyName) ? strategyDictionary[strategyName] : null;
        }
    }
}
