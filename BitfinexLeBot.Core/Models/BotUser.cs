using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitfinexLeBot.Core.Models
{
    public class BotUser
    {
        public virtual int BotUserId { get; protected set; }

        public string BotUserName { get; set; }

        public string ApiKey { get; set; }

        public string Secret { get; set; }


    }
}
