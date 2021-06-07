using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Sector.Api.Models;
using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Services
{
    public class DatabaseContext
    {
        public readonly IMongoCollection<SectorEntity> Sector;

        public DatabaseContext(IOptions<DatabaseConfig> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            var database = client.GetDatabase(config.Value.DatabaseName);

            var sectorCollection = database.GetCollection<SectorEntity>("Sectors");
            SectorCollectionManager.CreateIndex(sectorCollection);

            this.Sector = sectorCollection;
        }
    }
}