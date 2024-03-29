using Bitfinex.Net.Objects.Models.V1;
using BitfinexLeBot.Core.Interfaces;
using BitfinexLeBot.Core.Models;
using BitfinexLeBot.Core.Models.Strategy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Services.Strategy;

public class BasicStrategy : IStrategy
{
    public StrategyResult Execute(IQuoteSource quoteSource, IFundingOperate fundingOperate, BotUser botUser, string fundingSymbol, object strategyConfigJson)
    {
        StrategyResult result = new StrategyResult()
        {
            UserId = botUser.BotUserId.ToString(),
            StrategyName = GetType().Name,
            ResultCode = 0
        };

        string strategyConfigJsonStr = "";
        if (strategyConfigJson != null)
        {
            strategyConfigJsonStr = strategyConfigJson.ToString() ?? "";
        }

        BasicStrategyConfig? config = JsonConvert.DeserializeObject<BasicStrategyConfig>(strategyConfigJsonStr);
        if (config == null)
        {
            result.ResultCode = -1;
            result.ErrorMessage = "cannot get config";
            return result;
        }

        decimal offeringBalance = 0;
        if (config.UpdateOfferingEveryRun)
        {
            var fundingState = fundingOperate.GetFundingState(botUser, fundingSymbol);
            if (fundingState?.FundingOffers.Count > 0)
            {
                offeringBalance = fundingState.TotalOfferingAmount;
                var cancelSignals = fundingOperate.CancelAllFundingOffers(botUser, fundingSymbol);
                result.Signals.AddRange(cancelSignals);
            }
        }

        var fundingBalance = fundingOperate.GetFundingBalance(botUser, fundingSymbol);
        if (fundingBalance != null)
        {
            var book = quoteSource.GetFundingBook($"f{fundingSymbol}");

            decimal totalAvailableBalance = fundingBalance.AvailableBalance + offeringBalance;
            if (totalAvailableBalance > 50)
            {
                var signal = newOfferAtFirstAskAsync(fundingOperate, botUser, fundingSymbol, totalAvailableBalance, config, book);
                if (signal != null) result.Signals.Add(signal);
            }
        }
        return result;
    }


    private static BitfinexOffer? newOfferAtFirstAskAsync(IFundingOperate fundingOperate, BotUser botUser, string fundingSymbol, decimal amount, BasicStrategyConfig config, BitfinexFundingBook book)
    {
        BitfinexOffer sinal = new BitfinexOffer();

        if (book.Asks == null) return null;

        var offerAmount = StrategyHelper.GetFloorValue(amount, 5);
        var rate = StrategyHelper.GetFloorValue(book.Asks.ToList()[config.FixedAskIndex].Price, 4);
        int period = rate > config.CrazyRate ? config.CrazyRatePeriod : config.FixedPeriod;

        if (rate > config.MinRate)
        {
            sinal = fundingOperate.NewOffer(botUser, fundingSymbol, offerAmount, rate, period);
        }
        else
        {
            if (!config.DoNotOfferWhenUnderMinRate)
            {
                sinal = fundingOperate.NewOffer(botUser, fundingSymbol, offerAmount, config.MinRate, period);
            }
        }
        return sinal;
    }



}
