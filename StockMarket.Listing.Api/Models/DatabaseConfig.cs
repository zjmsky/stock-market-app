namespace StockMarket.Listing.Api.Models
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string SeedPolicy { get; set; }
    }
}