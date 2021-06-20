using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Models
{
    public class CompanyIntegrationEvent
    {
        public string EventType { get; set; }
        public string CompanyCode { get; set; }
        
        public bool IsUpdation() => EventType == Models.EventType.Update;
        public bool IsDeletion() => EventType == Models.EventType.Delete;

        public CompanyEntity ToEntity()
        {
            return new CompanyEntity
            {
                CompanyCode = CompanyCode,
            };
        }
    }
}