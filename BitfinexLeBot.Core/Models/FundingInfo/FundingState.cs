using Bitfinex.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Models.FundingInfo;

public class FundingState
{
    public decimal WeightedAvgProvidedRate { get; set; }
    public decimal WeightedAvgProvidedDayRate { get { return WeightedAvgProvidedRate / 365; } set { } }
    public decimal WeightedAvgProvidedDuration { get; set; }

    public decimal TotalProvidedAmount
    {
        get
        {
            decimal sum = 0;
            foreach (var credit in FundingCredits)
                sum += credit.Quantity;
            return sum;
        }
    }

    public decimal TotalOfferingAmount
    {
        get
        {
            decimal sum = 0;
            foreach (var offer in FundingOffers)
                sum += offer.Quantity;
            return sum;
        }
    }

    public List<BitfinexFundingCredit> FundingCredits { get; set; } = new List<BitfinexFundingCredit>();

    public List<BitfinexFundingOffer> FundingOffers { get; set; } = new List<BitfinexFundingOffer>();


}
