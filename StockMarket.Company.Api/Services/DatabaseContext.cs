using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Company.Api.Models;

namespace StockMarket.Company.Api.Services
{
    public class DatabaseContext
    {
        private readonly DatabaseConfig _config;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;

        public readonly IMongoCollection<Entities.Exchange> Exchanges;
        public readonly IMongoCollection<Entities.Sector> Sectors;
        public readonly IMongoCollection<Entities.Company> Companies;
        public readonly IMongoCollection<Entities.Listing> Listings;

        public DatabaseContext(IOptions<DatabaseConfig> config)
        {
            var configVal = config.Value;

            var client = new MongoClient(configVal.ConnectionString);
            var database = client.GetDatabase(configVal.DatabaseName);

            var exchangeCollection = database.GetCollection<Entities.Exchange>("Exchanges");
            Entities.ExchangeConstraintBuilder.Add(exchangeCollection);

            var sectorCollection = database.GetCollection<Entities.Sector>("Companies");
            Entities.SectorConstraintBuilder.Add(sectorCollection);

            var companyCollection = database.GetCollection<Entities.Company>("Companies");
            Entities.CompanyConstraintBuilder.Add(companyCollection);

            var listingCollection = database.GetCollection<Entities.Listing>("Listings");
            Entities.ListingConstraintBuilder.Add(listingCollection);
            
            _config = configVal;
            _client = client;
            _db = database;
            this.Exchanges = exchangeCollection;
            this.Sectors = sectorCollection;
            this.Companies = companyCollection;
            this.Listings = listingCollection;
        }
    }
}