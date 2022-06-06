using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.FundingInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Interfaces
{
    public interface IFundingInfo
    {

        Task<FundingState> GetFundingStateAsync(UserStrategy userStrategy);

        Task<FundingBalance> GetFundingBalanceAsync(UserStrategy userStrategy);

       Task<FundingPerformance> GetFundingPerformanceAsync(UserStrategy userStrategy);

    }
}
