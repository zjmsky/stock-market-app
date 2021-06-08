using EasyNetQ;
using MongoDB.Bson;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
{
    [Queue("SectorIntegrationEvents")]
    public interface ISectorIntegrationEvent
    {
        string Id { get; set; }
    }

    public class SectorCreationEvent : ISectorIntegrationEvent
    {
        public string Id { get; set; }
        public string SectorCode { get; set; }

        public SectorEntity ToEntity()
        {
            return new SectorEntity
            {
                Id = new ObjectId(Id),
                SectorCode = SectorCode,
            };
        }
    }

    public class SectorDeletionEvent : ISectorIntegrationEvent
    {
        public string Id { get; set; }

        public ObjectId ToId()
        {
            return new ObjectId(Id);
        }
    }
}