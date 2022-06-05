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
        void InitializeBot();

        bool RegisterUserStrategy(UserStrategy userStrategy, string strategyConfigJson);

        void UnregisterUserStrategy(int userStrategyId);

        List<UserStrategy> GetRegisteredUserStrategy();




    }
}
