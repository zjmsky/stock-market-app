namespace StockMarket.Auth.Api.Models
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
        public string DeviceId { get; set; }
    }
}
