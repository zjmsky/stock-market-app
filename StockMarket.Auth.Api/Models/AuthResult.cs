using System;

namespace StockMarket.Auth.Api.Models
{
    public class AuthResult
    {
        public readonly string AccessToken;
        public readonly string RefreshToken;
        public readonly string Error;

        private AuthResult(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Error = String.Empty;
        }

        private AuthResult(string error)
        {
            AccessToken = String.Empty;
            RefreshToken = String.Empty;
            Error = error;
        }

        public static AuthResult Success(string accessToken, string refreshToken)
        {
            return new AuthResult(accessToken, refreshToken);
        }

        public static AuthResult Failure(string message)
        {
            return new AuthResult(message);
        }

        public bool IsSuccess() => Error == String.Empty;
        public bool IsFailure() => Error != String.Empty;

        public object ToJson()
        {
            if (Error == String.Empty)
                return new { accessToken = AccessToken, refreshToken = RefreshToken };
            else
                return new { error = Error };
        }
    }
}
