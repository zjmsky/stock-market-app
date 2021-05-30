using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Auth.Api.Models;

namespace StockMarket.Auth.Api.Data
{
    public class AppDbContext
    {
        private readonly DatabaseConfig _config;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;

        public readonly IMongoCollection<Entities.User> Users;

        public AppDbContext(IOptions<DatabaseConfig> config)
        {
            var configVal = config.Value;

            var client = new MongoClient(configVal.ConnectionString);
            var database = client.GetDatabase(configVal.DatabaseName);

            var userCollection = database.GetCollection<Entities.User>("Users");
            AddUserConstraints(userCollection);

            _config = configVal;
            _client = client;
            _db = database;
            Users = userCollection;
        }

        private static void AddUserConstraints(IMongoCollection<Entities.User> collection)
        {
            var indexBuilder = Builders<Entities.User>.IndexKeys;

            var nameIndexKey = indexBuilder.Ascending(e => e.Username);
            var nameIndexOpts = new CreateIndexOptions() { Unique = true };
            var nameIndexModel = new CreateIndexModel<Entities.User>(nameIndexKey, nameIndexOpts);
            collection.Indexes.CreateOne(nameIndexModel);

            var emailIndexKey = indexBuilder.Ascending(e => e.Email);
            var emailIndexOpts = new CreateIndexOptions() { Unique = true };
            var emailIndexModel = new CreateIndexModel<Entities.User>(emailIndexKey, emailIndexOpts);
            collection.Indexes.CreateOne(emailIndexModel);
        }
    }
}
