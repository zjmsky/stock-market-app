using EasyNetQ;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Models
{
    [Queue("SectorIntegrationEvents")]
    public class SectorIntegrationEvent
    {
        public string Type { get; private set; }
        public string SectorCode { get; private set; }

        public bool IsUpdationEvent() => Type == "update";
        public bool IsDeletionEvent() => Type == "delete";

        public SectorEntity ToEntity()
        {
            return new SectorEntity
            {
                SectorCode = SectorCode,
            };
        }
    }
}