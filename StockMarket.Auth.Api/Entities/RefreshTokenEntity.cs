using System;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Auth.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class RefreshTokenEntity
    {
        public string Token { get; set; }
        public string IpAddress { get; set; }
        public DateTime ExpiresAt { get; set; }

        public RefreshTokenEntity(string token, string ipAddress)
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

}