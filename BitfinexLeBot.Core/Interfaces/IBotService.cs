using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.FundingInfo;

namespace BitfinexLeBot.Core.Interfaces;

public interface IBotService : IFundingOperate
{
    void InitializeBot();

    bool RegisterUserStrategy(UserStrategy userStrategy, object? strategyConfigJson);

    void UnregisterUserStrategy(int userStrategyId);

    List<UserStrategy> GetRegisteredUserStrategy();

    void RunStep();

    FundingState GetFundingState(BotUser user, string fundinSymbol);
}
