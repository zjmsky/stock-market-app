using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Auth.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class RefreshToken
    {
        public string Token { get; set; }
        public string IpAddress { get; set; }
        public DateTime ExpiresAt { get; set; }

        public RefreshToken(string token, string ipAddress)
        {
            Token = token;
            IpAddress = ipAddress;
            ExpiresAt = DateTime.UtcNow.AddDays(7);
        }

        public bool HasExpired()
        {
            return ExpiresAt < DateTime.UtcNow;
        }
    }

    public enum UserRole
    {
        General,
        Admin,
        Superuser,
    }

    [BsonIgnoreExtraElements]
    public class User
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; } // TODO: salt + hash
        public UserRole Role { get; set; }

        public string Email { get; set; }
        public bool IsVerified { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            var nameIsValid = Regex.IsMatch(Username, @"^\w{1,30}$");
            if (nameIsValid == false) result.Add("username", "invalid username");

            var emailIsValid = Regex.IsMatch(Email, @"@.*?\.");
            if (emailIsValid == false) result.Add("email", "invalid email");

            return result;
        }
    }
}
