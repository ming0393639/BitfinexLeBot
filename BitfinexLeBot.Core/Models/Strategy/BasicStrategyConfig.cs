using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Models.Strategy
{
    public class BasicStrategyConfig
    {
        public virtual int BotUserId { get; protected set; }

        public string FundingSymbol { get; set; }

        public bool UpdateOfferingEveryRun { get; set; } = true;

        public int FixedPeriod { get; set; } = 2;

        public int FixedAskIndex { get; set; } = 0;

        public decimal MinRate { get; set; } = 8;

        public bool DoNotOfferWhenUnderMinRate { get; set; } = true;

        public decimal CrazyRate { get; set; } = 25;

        public int CrazyRatePeriod { get; set; } = 30;
    }
}
