using EasyNetQ;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
{
    [Queue("ListingIntegrationEvents")]
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