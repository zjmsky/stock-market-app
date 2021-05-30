namespace StockMarket.Auth.Api.Models
{
    public class AuthConfig
    {
        public string SecretKey { get; set; }
    }

    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
