using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Models.Strategy
{
    public class RecentHighStrategyConfig
    {
        public bool UpdateOfferingEveryRun { get; set; } = true;

        public int FixedPeriod { get; set; } = 2;

        public int RecentHighTradeCount { get; set; } = 20;

        public int RecentHighKCount { get; set; } = 8;

        public decimal OrderRateFactor { get; set; } = 1.0M;

        public decimal MinRate { get; set; } = 8;

        public bool DoNotOfferWhenUnderMinRate { get; set; } = true;

        public decimal CrazyReservedSharePercent { get; set; } = 0;

        public decimal CrazyRate { get; set; } = 300;

        public int CrazyRatePeriod { get; set; } = 30;



    }
}
