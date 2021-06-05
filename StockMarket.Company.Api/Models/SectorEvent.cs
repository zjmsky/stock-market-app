using EasyNetQ;

namespace StockMarket.Company.Api.Models
{
    [Queue("SectorIntegrationEvents")]
    public interface ISectorIntegrationEvent
    {
        string SectorCode { get; set; }
    }
    
    public class SectorCreatedEvent: ISectorIntegrationEvent
    {
        public string SectorCode { get; set; }
        public string Name { get; set; }
    }

    public class SectorDeletedEvent: ISectorIntegrationEvent
    {
        public string SectorCode { get; set; }
    }
}