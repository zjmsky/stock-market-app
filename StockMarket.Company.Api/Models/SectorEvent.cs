using EasyNetQ;

namespace StockMarket.Company.Api.Models
{
    [Queue("SectorIntegrationEvents")]
    public interface ISectorIntegrationEvent
    {
        string Id { get; set; }
    }
    
    public class SectorCreatedEvent: ISectorIntegrationEvent
    {
        public string Id { get; set; }
        public string SectorCode { get; set; }
        public string Name { get; set; }
    }

    public class SectorDeletedEvent: ISectorIntegrationEvent
    {
        public string Id { get; set; }
    }
}