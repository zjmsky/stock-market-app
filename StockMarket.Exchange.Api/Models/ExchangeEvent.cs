using EasyNetQ;
using MongoDB.Bson;
using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Models
{
    [Queue("ExchangeIntegrationEvents")]
    public interface IExchangeIntegrationEvent
    {
        string Id { get; set; }
    }

    public class ExchangeCreationEvent : IExchangeIntegrationEvent
    {
        public string Id { get; set; }
        public string ExchangeCode { get; set; }

        public static ExchangeCreationEvent FromEntity(ExchangeEntity exchange)
        {
            return new ExchangeCreationEvent
            {
                Id = exchange.Id.ToString(),
                ExchangeCode = exchange.ExchangeCode,
            };
        }
    }

    public class ExchangeDeletionEvent : IExchangeIntegrationEvent
    {
        public string Id { get; set; }

        public static ExchangeDeletionEvent FromId(ObjectId id)
        {
            return new ExchangeDeletionEvent
            {
                Id = id.ToString()
            };
        }
    }
}