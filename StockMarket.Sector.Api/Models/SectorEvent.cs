using EasyNetQ;
using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Models
{
    [Queue("SectorIntegrationEvents")]
    public class SectorIntegrationEvent
    {
        string Type { get; set; }
        string SectorCode { get; set; }

        public static SectorIntegrationEvent Update(SectorEntity sector)
        {
            return new SectorIntegrationEvent
            {
                Type = "update",
                SectorCode = sector.SectorCode
            };
        }

        public static SectorIntegrationEvent Delete(SectorEntity sector)
        {
            return new SectorIntegrationEvent
            {
                Type = "delete",
                SectorCode = sector.SectorCode
            };
        }
    }
}