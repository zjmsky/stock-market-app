using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Auth.Api.Entities
{
    public enum UserRole
    {
        General,
        Admin,
        Superuser,
    }

    [BsonIgnoreExtraElements]
    public class UserEntity
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; } // TODO: salt + hash
        public UserRole Role { get; set; }

        public string Email { get; set; }
        public bool IsVerified { get; set; }

        public List<RefreshTokenEntity> RefreshTokens { get; set; }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (Regex.IsMatch(Username, @"^\w{1,30}$") == false)
                result.Add("username", "invalid username");

            if (Regex.IsMatch(Email, @"@.*?\.") == false)
                result.Add("email", "invalid email");

            return result;
        }
    }

    public static class UserCollectionManager
    {
        public static void CreateIndex(IMongoCollection<UserEntity> collection)
        {
            var indexBuilder = Builders<UserEntity>.IndexKeys;

            var nameIndexKey = indexBuilder.Ascending(e => e.Username);
            var nameIndexOpts = new CreateIndexOptions { Unique = true };
            var nameIndexModel = new CreateIndexModel<UserEntity>(nameIndexKey, nameIndexOpts);
            collection.Indexes.CreateOne(nameIndexModel);

            var emailIndexKey = indexBuilder.Ascending(e => e.Email);
            var emailIndexOpts = new CreateIndexOptions { Unique = true };
            var emailIndexModel = new CreateIndexModel<UserEntity>(emailIndexKey, emailIndexOpts);
            collection.Indexes.CreateOne(emailIndexModel);
        }
    }
}
