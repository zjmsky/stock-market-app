using EasyNetQ;
using MongoDB.Bson;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
{
    [Queue("CompanyIntegrationEvents")]
    public interface ICompanyIntegrationEvent
    {
        string Id { get; set; }
    }

    public class CompanyCreationEvent : ICompanyIntegrationEvent
    {
        public string Id { get; set; }
        public string CompanyCode { get; set; }

        public static CompanyCreationEvent FromEntity(CompanyEntity company)
        {
            return new CompanyCreationEvent
            {
                Id = company.Id.ToString(),
                CompanyCode = company.CompanyCode,
            };
        }
    }

    public class CompanyDeletionEvent : ICompanyIntegrationEvent
    {
        public string Id { get; set; }

        public static CompanyDeletionEvent FromId(ObjectId id)
        {
            return new CompanyDeletionEvent { Id = id.ToString() };
        }
    }
}