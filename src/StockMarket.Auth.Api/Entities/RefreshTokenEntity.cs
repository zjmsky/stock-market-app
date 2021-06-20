using System;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Auth.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class RefreshTokenEntity
    {
        public string DeviceId;
        public string Token;
        public DateTime ExpiresAt;

        public RefreshTokenEntity(string deviceId, string token)
        {
            DeviceId = deviceId;
            Token = token;
            ExpiresAt = DateTime.UtcNow.AddDays(7);
        }

        public bool HasExpired()
        {
            return ExpiresAt < DateTime.UtcNow;
        }
    }

}