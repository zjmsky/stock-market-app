using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Models
{
    public class SectorIntegrationEvent
    {
        public string EventType { get; set; }
        public string SectorCode { get; set; }

        public SectorIntegrationEvent(string type, SectorEntity sector)
        {
            EventType = type;
            SectorCode = sector.SectorCode;
        }
    }
}