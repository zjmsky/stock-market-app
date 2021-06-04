using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Sector.Api.Models;

namespace StockMarket.Sector.Api.Services
{
    public class DatabaseContext
    {
        public readonly IMongoCollection<Entities.Sector> Sector;

        public DatabaseContext(IOptions<DatabaseConfig> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            var database = client.GetDatabase(config.Value.DatabaseName);

            var sectorCollection = database.GetCollection<Entities.Sector>("Sectors");
            Entities.SectorConstraintBuilder.Add(sectorCollection);

            this.Sector = sectorCollection;
        }
    }
}