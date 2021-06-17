using EasyNetQ;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
{
    [Queue("CompanyIntegrationEvents")]
    public class CompanyIntegrationEvent
    {
        string Type { get; set; }
        string CompanyCode { get; set; }
        string SectorCode { get; set; }
        
        public static CompanyIntegrationEvent Update(CompanyEntity company)
        {
            return new CompanyIntegrationEvent
            {
                Type = "update",
                CompanyCode = company.CompanyCode,
                SectorCode = company.SectorCode
            };
        }

        public static CompanyIntegrationEvent Delete(CompanyEntity company)
        {
            return new CompanyIntegrationEvent
            {
                Type = "delete",
                CompanyCode = company.CompanyCode,
                SectorCode = company.SectorCode
            };
        }
    }
}
