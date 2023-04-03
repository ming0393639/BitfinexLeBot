using BitfinexLeBot.Core.Models;

namespace BitfinexLeBot.Core.Interfaces;

public interface IBotService : IFundingOperate
{
    void InitializeBot();

    bool RegisterUserStrategy(UserStrategy userStrategy, object? strategyConfigJson);

    void UnregisterUserStrategy(int userStrategyId);

    List<UserStrategy> GetRegisteredUserStrategy();

    void RunStep();

}
