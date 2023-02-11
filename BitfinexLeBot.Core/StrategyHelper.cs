using Bitfinex.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core;

public class StrategyHelper
{
    public static decimal GetFloorValue(decimal value, int decimalPoint)
    {
        decimal dp = (decimal)Math.Pow(10, decimalPoint);
        return Math.Floor(value * dp) / dp;
    }

    public static DateTime CalKLinesStartTime(KlineInterval timeFrame, DateTime endTime, int amount) 
        => endTime.AddMinutes(((int) timeFrame) * -amount);

}
