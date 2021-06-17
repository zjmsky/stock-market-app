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
            var configVal = config.Value;

            var client = new MongoClient(configVal.ConnectionString);
            var database = client.GetDatabase(configVal.DatabaseName);

            var sectorCollection = database.GetCollection<SectorEntity>("Sectors");
            SectorCollectionManager.CreateIndex(sectorCollection);

            var companyCollection = database.GetCollection<CompanyEntity>("Companies");
            CompanyCollectionManager.CreateIndex(companyCollection);

            var listingCollection = database.GetCollection<ListingEntity>("Listings");
            ListingCollectionManager.CreateIndex(listingCollection);
            
            this.Sectors = sectorCollection;
            this.Companies = companyCollection;
            this.Listings = listingCollection;
        }
    }
}