using EasyNetQ;

namespace StockMarket.Exchange.Api.Models
{
    [Queue("ExchangeIntegrationEvents")]
    public interface IExchangeIntegrationEvent
    {
        string ExchangeCode { get; set; }
    }

    public class ExchangeCreatedIntegrationEvent : IExchangeIntegrationEvent
    {
        public string ExchangeCode { get; set; }
        public string Name { get; set; }

        public ExchangeCreatedIntegrationEvent(Entities.Exchange exchange)
        {
            ExchangeCode = exchange.ExchangeCode;
            Name = exchange.Name;
        }
    }

    public class ExchangeDeletedIntegrationEvent : IExchangeIntegrationEvent
    {
        public string ExchangeCode { get; set; }

        public ExchangeDeletedIntegrationEvent(string code)
        {
            ExchangeCode = code;
        }
    }
}