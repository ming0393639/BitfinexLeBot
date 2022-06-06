using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Models.FundingInfo
{
    public class ProfitInfo
    {
        public decimal Profit { get; set; }
        public DateTime Date { get; set; }


        public ProfitInfo(decimal profit, DateTime date)
        {
            Profit = profit;
            Date = date;
        }
    }
}
