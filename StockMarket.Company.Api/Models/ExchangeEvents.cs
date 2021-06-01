using EasyNetQ;

namespace StockMarket.Company.Api.Models
{
    [Queue("ExchangeIntegrationEvents")]
    public interface IExchangeIntegrationEvent
    {
        string Id { get; set; }
    }
    
    public class ExchangeCreatedEvent: IExchangeIntegrationEvent
    {
        public string Id { get; set; }
        public string ExchangeCode { get; set; }
        public string Name { get; set; }
    }

    public class ExchangeDeletedEvent: IExchangeIntegrationEvent
    {
        public string Id { get; set; }
    }
}