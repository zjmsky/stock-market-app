using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Auth.Api.Models;
using StockMarket.Auth.Api.Entities;

namespace StockMarket.Auth.Api.Services
{
    public class DatabaseContext
    {
        public readonly IMongoCollection<UserEntity> Users;

        public DatabaseContext(IOptions<DatabaseConfig> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            var database = client.GetDatabase(config.Value.DatabaseName);

            var userCollection = database.GetCollection<UserEntity>("Users");
            UserCollectionManager.CreateIndex(userCollection);
            UserCollectionManager.Seed(userCollection, config.Value.SeedPolicy);
            
            this.Users = userCollection;
        }

    }
}
