using EasyNetQ;
using MongoDB.Bson;
using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Models
{
    [Queue("ListingIntegrationEvents")]
    public interface IListingIntegrationEvent
    {
        string Id { get; set; }
    }

    public class ListingCreationEvent : IListingIntegrationEvent
    {
        public string Id { get; set; }
        public string ExchangeCode { get; set; }
        public string TickerSymbol { get; set; }

        public ListingEntity ToEntity()
        {
            return new ListingEntity
            {
                Id = new ObjectId(Id),
                ExchangeCode = ExchangeCode,
                TickerSymbol = TickerSymbol
            };
        }
    }

    public class ListingDeletionEvent : IListingIntegrationEvent
    {
        public string Id { get; set; }

        public ObjectId ToId()
        {
            return new ObjectId(Id);
        }
    }
}