using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Models
{
    public class ExchangeIntegrationEvent
    {
        public string EventType { get; set; }
        public string ExchangeCode { get; set; }

        public ExchangeIntegrationEvent(string type, ExchangeEntity exchange)
        {
            EventType = type;
            ExchangeCode = exchange.ExchangeCode;
        }
    }
}