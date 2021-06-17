using EasyNetQ;
using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Models
{
    [Queue("CompanyIntegrationEvents")]
    public class CompanyIntegrationEvent
    {
        public string Type { get; private set; }
        public string CompanyCode { get; private set; }
        public string SectorCode { get; private set; }
        
        public bool IsUpdationEvent() => Type == "update";
        public bool IsDeletionEvent() => Type == "delete";

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