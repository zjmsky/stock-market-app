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
            var configVal = config.Value;

            var client = new MongoClient(configVal.ConnectionString);
            var database = client.GetDatabase(configVal.DatabaseName);

            var userCollection = database.GetCollection<UserEntity>("Users");
            UserCollectionManager.CreateIndex(userCollection);

            this.Users = userCollection;
        }

    }
}
