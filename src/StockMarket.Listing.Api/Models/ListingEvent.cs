using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Models
{
    public class ListingIntegrationEvent
    {
        public string EventType { get; set; }
        public string ExchangeCode { get; set; }
        public string TickerSymbol { get; set; }

        public ListingIntegrationEvent(string type, ListingEntity listing)
        {
            EventType = type;
            ExchangeCode = listing.ExchangeCode;
            TickerSymbol = listing.TickerSymbol;
        }
    }
}