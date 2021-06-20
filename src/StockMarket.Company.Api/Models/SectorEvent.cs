using EasyNetQ;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
{
    [Queue("SectorIntegrationEvents")]
    public class SectorIntegrationEvent
    {
        public string EventType { get; set; }
        public string SectorCode { get; set; }

        public bool IsUpdation() => EventType == Models.EventType.Update;
        public bool IsDeletion() => EventType == Models.EventType.Delete;

        public SectorEntity ToEntity()
        {
            return new SectorEntity
            {
                SectorCode = SectorCode,
            };
        }
    }
}