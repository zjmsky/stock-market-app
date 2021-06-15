using System;

namespace StockMarket.Auth.Api.Models
{
    public class AuthResponse
    {
        public readonly string AccessToken;
        public readonly string RefreshToken;
        public readonly string Error;

        private AuthResponse(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Error = String.Empty;
        }

        private AuthResponse(string error)
        {
            AccessToken = String.Empty;
            RefreshToken = String.Empty;
            Error = error;
        }

        public static AuthResponse Success(string accessToken, string refreshToken)
        {
            return new AuthResponse(accessToken, refreshToken);
        }

        public static AuthResponse Failure(string message)
        {
            return new AuthResponse(message);
        }

        public bool IsSuccess() => Error == String.Empty;
        public bool IsFailure() => Error != String.Empty;
    }
}
