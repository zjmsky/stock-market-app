namespace StockMarket.Exchange.Api.Models
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class EventBusConfig
    {
        public string ConnectionString { get; set; }
    }
}