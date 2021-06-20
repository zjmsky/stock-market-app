using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Listing.Api.Models;
using StockMarket.Listing.Api.Entities;

namespace StockMarket.Listing.Api.Services
{
    public class DatabaseContext
    {
        public readonly IMongoCollection<ExchangeEntity> Exchanges;
        public readonly IMongoCollection<CompanyEntity> Companies;
        public readonly IMongoCollection<ListingEntity> Listings;
        public readonly IMongoCollection<IpoEntity> Ipos;
        public readonly IMongoCollection<PriceEntity> Prices;

        public DatabaseContext(IOptions<DatabaseConfig> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            var database = client.GetDatabase(config.Value.DatabaseName);

            var exchangeCollection = database.GetCollection<ExchangeEntity>("Exchanges");
            ExchangeCollectionManager.CreateIndex(exchangeCollection);
            ExchangeCollectionManager.Seed(exchangeCollection, config.Value.SeedPolicy);

            var companyCollection = database.GetCollection<CompanyEntity>("Companies");
            CompanyCollectionManager.CreateIndex(companyCollection);
            CompanyCollectionManager.Seed(companyCollection, config.Value.SeedPolicy);
            
            var listingCollection = database.GetCollection<ListingEntity>("Listings");
            ListingCollectionManager.CreateIndex(listingCollection);
            ListingCollectionManager.Seed(listingCollection, config.Value.SeedPolicy);

            var ipoCollection = database.GetCollection<IpoEntity>("Ipos");
            IpoCollectionManager.CreateIndex(ipoCollection);

            var priceCollection = database.GetCollection<PriceEntity>("Prices");
            PriceCollectionManager.CreateIndex(priceCollection);

            this.Exchanges = exchangeCollection;
            this.Companies = companyCollection;
            this.Listings = listingCollection;
            this.Ipos = ipoCollection;
            this.Prices = priceCollection;
        }
    }
}
