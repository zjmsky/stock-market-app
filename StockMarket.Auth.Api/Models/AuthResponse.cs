namespace StockMarket.Auth.Api.Models
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Error { get; set; }

        public static AuthResponse Success(string accessToken, string refreshToken)
        {
            return new AuthResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Error = string.Empty,
            };
        }

        public static AuthResponse Failure(string message)
        {
            return new AuthResponse()
            {
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                Error = message,
            };
        }

        public bool IsSuccess() => Error == string.Empty;
        public bool IsFailure() => Error != string.Empty;
    }
}
