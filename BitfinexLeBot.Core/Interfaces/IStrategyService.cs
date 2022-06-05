using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Interfaces
{
    public interface IStrategyService
    {

        IStrategy GetStrategy(string strategyName);




    }
}
