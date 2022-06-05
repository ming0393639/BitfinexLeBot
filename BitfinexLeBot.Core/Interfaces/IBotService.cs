using BitfinexLeBot.Core.Models;

namespace BitfinexLeBot.Core.Interfaces
{
    public interface IBotService : IFundingInfo
    {
        void InitializeBot();

        bool RegisterUserStrategy(UserStrategy userStrategy, string strategyConfigJson);

        void UnregisterUserStrategy(int userStrategyId);

        List<UserStrategy> GetRegisteredUserStrategy();



    }
}
