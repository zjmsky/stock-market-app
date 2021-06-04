using EasyNetQ;

namespace StockMarket.Company.Api.Models
{
    [Queue("ExchangeIntegrationEvents")]
    public interface IExchangeIntegrationEvent
    {
        string ExchangeCode { get; set; }
    }
    
    public class ExchangeCreatedIntegrationEvent: IExchangeIntegrationEvent
    {
        public string ExchangeCode { get; set; }
        public string Name { get; set; }
    }

    public class ExchangeDeletedIntegrationEvent: IExchangeIntegrationEvent
    {
        public string ExchangeCode { get; set; }
    }
}