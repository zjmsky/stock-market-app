using EasyNetQ;
using MongoDB.Bson;

namespace StockMarket.Sector.Api.Models
{
    [Queue("SectorIntegrationEvents")]
    public interface ISectorIntegrationEvent
    {
        string Id { get; set; }
    }

    public class SectorCreatedIntegrationEvent : ISectorIntegrationEvent
    {
        public string Id { get; set; }
        public string SectorCode { get; set; }

        public static SectorCreatedIntegrationEvent FromEntity(Entities.Sector sector)
        {
            return new SectorCreatedIntegrationEvent
            {
                Id = sector.Id.ToString(),
                SectorCode = sector.SectorCode,
            };
        }
    }

    public class SectorDeletedIntegrationEvent : ISectorIntegrationEvent
    {
        public string Id { get; set; }

        public static SectorDeletedIntegrationEvent FromId(ObjectId id)
        {
            return new SectorDeletedIntegrationEvent
            {
                Id = id.ToString(),
            };
        }
    }
}