using Bitfinex.Net.Objects.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace BitfinexLeBot.Core.Models.FundingInfo;

[SwaggerSchema("My parameter description")]
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
