using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Models
{
    public class UserStrategy
    {

        public virtual int UserStrategyId { get; protected set; }

        public BotUser User { get; set; }

        public bool Active { get; set; } = true;

        public string StrategyName { get; set; }

        public string FundingSymbol { get; set; }

        [JsonIgnore]
        public string StrategyConfigJson { get; set; }


    }
}
