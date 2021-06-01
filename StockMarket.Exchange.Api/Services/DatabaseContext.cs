using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockMarket.Exchange.Api.Models;

namespace StockMarket.Exchange.Api.Services
{
    public class DatabaseContext
    {
        public readonly IMongoCollection<Entities.Exchange> Exchange;

        public DatabaseContext(IOptions<DatabaseConfig> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            var database = client.GetDatabase(config.Value.DatabaseName);

            var exchangeCollection = database.GetCollection<Entities.Exchange>("Exchanges");
            Entities.ExchangeConstraintBuilder.Add(exchangeCollection);
            
            this.Exchange = exchangeCollection;
        }
    }
}
