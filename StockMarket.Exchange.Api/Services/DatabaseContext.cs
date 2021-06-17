using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Exchange.Api.Models;
using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Services
{
    public class DatabaseContext
    {
        public readonly IMongoCollection<ExchangeEntity> Exchanges;
        public readonly IMongoCollection<ListingEntity> Listings;

        public DatabaseContext(IOptions<DatabaseConfig> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            var database = client.GetDatabase(config.Value.DatabaseName);

            var exchangeCollection = database.GetCollection<ExchangeEntity>("Exchanges");
            ExchangeCollectionManager.CreateIndex(exchangeCollection);
            
            var listingCollection = database.GetCollection<ListingEntity>("Listings");
            ListingCollectionManager.CreateIndex(listingCollection);

            this.Exchanges = exchangeCollection;
            this.Listings = listingCollection;
        }
    }
}
