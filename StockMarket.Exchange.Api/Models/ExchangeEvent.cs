using EasyNetQ;
using MongoDB.Bson;

namespace StockMarket.Exchange.Api.Models
{
    [Queue("ExchangeIntegrationEvents")]
    public interface IExchangeIntegrationEvent
    {
        string Id { get; set; }
    }

    public class ExchangeCreatedIntegrationEvent : IExchangeIntegrationEvent
    {
        public string Id { get; set; }
        public string ExchangeCode { get; set; }

        public static ExchangeCreatedIntegrationEvent FromEntity(Entities.Exchange exchange)
        {
            return new ExchangeCreatedIntegrationEvent
            {
                Id = exchange.Id.ToString(),
                ExchangeCode = exchange.ExchangeCode,
            };
        }
    }

    public class ExchangeDeletedIntegrationEvent : IExchangeIntegrationEvent
    {
        public string Id { get; set; }

        public static ExchangeDeletedIntegrationEvent FromId(ObjectId id)
        {
            return new ExchangeDeletedIntegrationEvent
            {
                Id = id.ToString()
            };
        }
    }
}