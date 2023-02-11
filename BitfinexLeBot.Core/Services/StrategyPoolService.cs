using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Services.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services;

public class StrategyPoolService : IStrategyService
{
    private readonly Dictionary<string, IStrategy> _strategyDictionary = new();
    

    public StrategyPoolService()
    {
        _strategyDictionary.Add("Basic", new BasicStrategy());
        _strategyDictionary.Add("RecentHigh", new RecentHighStrategy());

    }

    public IStrategy? GetStrategy(string strategyName) 
        => _strategyDictionary.ContainsKey(strategyName) ? _strategyDictionary[strategyName] : null;
}
