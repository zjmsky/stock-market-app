using System;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Auth.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class RefreshTokenEntity
    {
        public string Token;
        public DateTime ExpiresAt;

        public RefreshTokenEntity(string token)
        {
            Token = token;
            ExpiresAt = DateTime.UtcNow.AddDays(7);
        }

        public bool HasExpired()
        {
            return ExpiresAt < DateTime.UtcNow;
        }
    }

}