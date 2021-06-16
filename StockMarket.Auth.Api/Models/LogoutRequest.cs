namespace StockMarket.Auth.Api.Models
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
        public string DeviceId { get; set; }
    }
}
