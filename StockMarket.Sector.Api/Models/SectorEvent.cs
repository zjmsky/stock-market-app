using EasyNetQ;
using MongoDB.Bson;
using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Models
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

        public static SectorCreationEvent FromEntity(SectorEntity sector)
        {
            return new SectorCreationEvent
            {
                Id = sector.Id.ToString(),
                SectorCode = sector.SectorCode,
            };
        }
    }

    public class SectorDeletionEvent : ISectorIntegrationEvent
    {
        public string Id { get; set; }

        public static SectorDeletionEvent FromId(ObjectId id)
        {
            return new SectorDeletionEvent
            {
                Id = id.ToString(),
            };
        }
    }
}