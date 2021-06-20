using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using StockMarket.Auth.Api.Models;

namespace StockMarket.Auth.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class UserEntity
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public string Username { get; set; }
        public string Password { private get; set; } // TODO: salt + hash
        public string Role { get; set; }

        public string Email { get; set; }
        public bool IsVerified { get; set; }

        public List<RefreshTokenEntity> RefreshTokens { get; set; }

        public bool CheckPassword(string password) => Password == password;

        private static UserEntity CreateUser(string username, string password, string email, string role)
        {
            return new UserEntity
            {
                Username = username,
                Password = password,
                Role = role,
                Email = email,
                IsVerified = true, // TODO: implement mail confirmations
                RefreshTokens = new List<RefreshTokenEntity>()
            };
        }

        public static UserEntity General(string username, string password, string email) =>
            CreateUser(username, password, email, UserRole.General);

        public static UserEntity Admin(string username, string password, string email) =>
            CreateUser(username, password, email, UserRole.Admin);

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

        public static void Seed(IMongoCollection<UserEntity> collection, string policy)
        {
            if (policy.ToLower() != "dev")
                return;
            
            if (collection.Find(x => true).Any() == false)
            {
                collection.InsertOne(UserEntity.General("person", "Pass@123", "person@test.com"));
                collection.InsertOne(UserEntity.Admin("admin", "Admin@123", "admin@test.com"));
            }
        }
    }
}
