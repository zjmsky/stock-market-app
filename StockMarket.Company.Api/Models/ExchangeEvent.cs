using EasyNetQ;
using MongoDB.Bson;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
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

        public ExchangeEntity ToEntity()
        {
            return new ExchangeEntity
            {
                Id = new ObjectId(Id),
                ExchangeCode = ExchangeCode,
            };
        }
    }

    public class ExchangeDeletionEvent : IExchangeIntegrationEvent
    {
        public string Id { get; set; }

        public ObjectId ToId()
        {
            return new ObjectId(Id);
        }
    }
}