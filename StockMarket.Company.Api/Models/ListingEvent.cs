using EasyNetQ;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
{
    [Queue("ListingIntegrationEvents")]
    public class ListingIntegrationEvent
    {
        public string Type { get; private set; }
        public string ExchangeCode { get; private set; }
        public string TickerSymbol { get; private set; }

        public bool IsUpdationEvent() => Type == "update";
        public bool IsDeletionEvent() => Type == "delete";

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