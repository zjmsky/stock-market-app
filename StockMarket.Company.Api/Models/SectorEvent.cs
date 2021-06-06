using EasyNetQ;
using MongoDB.Bson;

namespace StockMarket.Company.Api.Models
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

        public Entities.Sector IntoEntity()
        {
            return new Entities.Sector
            {
                Id = new ObjectId(Id),
                SectorCode = SectorCode,
            };
        }
    }

    public class SectorDeletedIntegrationEvent : ISectorIntegrationEvent
    {
        public string Id { get; set; }

        public ObjectId IntoId()
        {
            return new ObjectId(Id);
        }
    }
}