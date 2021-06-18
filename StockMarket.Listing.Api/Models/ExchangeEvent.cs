using EasyNetQ;
using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Models
{
    [Queue("ExchangeIntegrationEvents")]
    public class ExchangeIntegrationEvent
    {
        public string Type { get; private set; }
        public string ExchangeCode { get; private set; }

        public bool IsUpdationEvent() => Type == "update";
        public bool IsDeletionEvent() => Type == "delete";

        public ExchangeEntity ToEntity()
        {
            return new ExchangeEntity
            {
                ExchangeCode = ExchangeCode,
            };
        }
    }
}