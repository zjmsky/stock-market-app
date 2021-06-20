using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Models
{
    public class ExchangeIntegrationEvent
    {
        public string EventType { get; set; }
        public string ExchangeCode { get; set; }

        public bool IsUpdation() => EventType == Models.EventType.Update;
        public bool IsDeletion() => EventType == Models.EventType.Delete;

        public ExchangeEntity ToEntity()
        {
            return new ExchangeEntity
            {
                ExchangeCode = ExchangeCode,
            };
        }
    }
}