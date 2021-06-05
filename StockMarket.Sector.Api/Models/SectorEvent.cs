using EasyNetQ;

namespace StockMarket.Sector.Api.Models
{
    [Queue("SectorIntegrationEvents")]
    public interface ISectorIntegrationEvent
    {
        string SectorCode { get; set; }
    }

    public class SectorCreatedIntegrationEvent : ISectorIntegrationEvent
    {
        public string SectorCode { get; set; }
        public string Name { get; set; }

        public SectorCreatedIntegrationEvent(Entities.Sector sector)
        {
            SectorCode = sector.SectorCode;
            Name = sector.Name;
        }
    }

    public class SectorDeletedIntegrationEvent : ISectorIntegrationEvent
    {
        public string SectorCode { get; set; }

        public SectorDeletedIntegrationEvent(string code)
        {
            SectorCode = code;
        }
    }
}