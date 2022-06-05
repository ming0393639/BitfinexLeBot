using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Models.FundingInfo
{
    public class FundingPerformance
    {
        public List<ProfitInfo> Profits { get; set; } = new List<ProfitInfo>();

        public double TotalProfit
        {
            get
            {
                decimal sum = 0;
                foreach (var profit in Profits)
                    sum += profit.Profit;
                return (double)sum;
            }
        }

        public double GetTotalProfitRate(double initialCapital)
        {
            return TotalProfit / initialCapital;
        }

        public double GetAnnualProfitRate(double initialCapital, DateTime startTime)
        {
            double days = DateTime.Now.Subtract(startTime).TotalDays;
            double years = days / 365;
            double p = GetTotalProfitRate(initialCapital);
            return Math.Pow(p + 1, 1 / (years)) - 1;
        }



    }
}
