using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Company.Api.Models;
using StockMarket.Company.Api.Entities;

namespace StockMarket.Company.Api.Services
{
    public class DatabaseContext
    {
        public readonly IMongoCollection<SectorEntity> Sectors;
        public readonly IMongoCollection<CompanyEntity> Companies;
        public readonly IMongoCollection<ListingEntity> Listings;

        public DatabaseContext(IOptions<DatabaseConfig> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            var database = client.GetDatabase(config.Value.DatabaseName);

            var sectorCollection = database.GetCollection<SectorEntity>("Sectors");
            SectorCollectionManager.CreateIndex(sectorCollection);
            SectorCollectionManager.Seed(sectorCollection, config.Value.SeedPolicy);

            var companyCollection = database.GetCollection<CompanyEntity>("Companies");
            CompanyCollectionManager.CreateIndex(companyCollection);
            CompanyCollectionManager.Seed(companyCollection, config.Value.SeedPolicy);

            var listingCollection = database.GetCollection<ListingEntity>("Listings");
            ListingCollectionManager.CreateIndex(listingCollection);
            ListingCollectionManager.Seed(listingCollection, config.Value.SeedPolicy);
            
            this.Sectors = sectorCollection;
            this.Companies = companyCollection;
            this.Listings = listingCollection;
        }
    }
}