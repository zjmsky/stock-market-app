using System;

namespace StockMarket.Auth.Api.Models
{
    public class AuthSuccess
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public AuthSuccess(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }

    public class AuthFailure
    {
        public string Error { get; set; }
    
        public AuthFailure(string error)
        {
            Error = error;
        }
    }

    public class AuthResult
    {
        private AuthSuccess success;
        private AuthFailure failure;

        public bool IsSuccess() => failure == null;
        public bool IsFailure() => failure != null;

        public AuthSuccess GetValue() => success;
        public AuthFailure GetError() => failure;

        public static AuthResult Success(string accessToken, string refreshToken)
        {
            var result = new AuthResult();
            result.success = new AuthSuccess(accessToken, refreshToken);
            return result;
        }

        public static AuthResult Failure(string message)
        {
            var result = new AuthResult();
            result.failure = new AuthFailure(message);
            return result;
        }
    }
}
