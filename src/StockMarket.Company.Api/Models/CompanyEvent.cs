using EasyNetQ;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
{
    [Queue("CompanyIntegrationEvents")]
    public class CompanyIntegrationEvent
    {
        public string EventType { get; set; }
        public string CompanyCode { get; set; }
        public string SectorCode { get; set; }

        public CompanyIntegrationEvent(string type, CompanyEntity company)
        {
            EventType = type;
            CompanyCode = company.CompanyCode;
            SectorCode = company.SectorCode;
        }
    }
}
