using EasyNetQ;
using MongoDB.Bson;

namespace StockMarket.Company.Api.Models
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

        public Entities.Exchange IntoEntity()
        {
            return new Entities.Exchange
            {
                Id = new ObjectId(Id),
                ExchangeCode = ExchangeCode,
            };
        }
    }

    public class ExchangeDeletedIntegrationEvent : IExchangeIntegrationEvent
    {
        public string Id { get; set; }

        public ObjectId IntoId()
        {
            return new ObjectId(Id);
        }
    }
}