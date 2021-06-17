using EasyNetQ;
using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Models
{
    [Queue("ExchangeIntegrationEvents")]
    public class ExchangeIntegrationEvent
    {
        string Type { get; set; }
        string ExchangeCode { get; set; }

        public static ExchangeIntegrationEvent Update(ExchangeEntity exchange)
        {
            return new ExchangeIntegrationEvent
            {
                Type = "update",
                ExchangeCode = exchange.ExchangeCode
            };
        }

        public static ExchangeIntegrationEvent Delete(ExchangeEntity exchange)
        {
            return new ExchangeIntegrationEvent
            {
                Type = "delete",
                ExchangeCode = exchange.ExchangeCode
            };
        }
    }
}