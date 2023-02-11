using Bitfinex.Net.Objects.Models.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Models.Strategy
{
    public  class StrategyResult
    {
        public int ResultCode { get; set; }
        public string? StrategyName { get; set; }
        public string? UserId { get; set; }
        public List<BitfinexOffer> Sinals { get; set; } = new List<BitfinexOffer>();
        public string? ErrorMessage { get; set; }

    }
}
