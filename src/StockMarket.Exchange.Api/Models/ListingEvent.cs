using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Models
{
    public class ListingIntegrationEvent
    {
        public string EventType { get; set; }
        public string ExchangeCode { get; set; }
        public string TickerSymbol { get; set; }

        public bool IsUpdation() => EventType == Models.EventType.Update;
        public bool IsDeletion() => EventType == Models.EventType.Delete;

        public ListingEntity ToEntity()
        {
            return new ListingEntity
            {
                ExchangeCode = ExchangeCode,
                TickerSymbol = TickerSymbol
            };
        }
    }
}