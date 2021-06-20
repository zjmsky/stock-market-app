using EasyNetQ;
using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Models
{
    [Queue("CompanyIntegrationEvents")]
    public class CompanyIntegrationEvent
    {
        public string EventType { get; set; }
        public string CompanyCode { get; set; }
        public string SectorCode { get; set; }
        
        public bool IsUpdation() => EventType == Models.EventType.Update;
        public bool IsDeletion() => EventType == Models.EventType.Delete;

        public CompanyEntity ToEntity()
        {
            return new CompanyEntity
            {
                CompanyCode = CompanyCode,
                SectorCode = SectorCode
            };
        }
    }
}