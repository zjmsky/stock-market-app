using EasyNetQ;
using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Models
{
    [Queue("ListingIntegrationEvents")]
    public class ListingIntegrationEvent
    {
        string Type { get; set; }
        string ExchangeCode { get; set; }
        string TickerSymbol { get; set; }

        public static ListingIntegrationEvent Update(ListingEntity listing)
        {
            return new ListingIntegrationEvent
            {
                Type = "update",
                ExchangeCode = listing.ExchangeCode,
                TickerSymbol = listing.TickerSymbol
            };
        }

        public static ListingIntegrationEvent Delete(ListingEntity listing)
        {
            return new ListingIntegrationEvent
            {
                Type = "delete",
                ExchangeCode = listing.ExchangeCode,
                TickerSymbol = listing.TickerSymbol
            };
        }
    }
}