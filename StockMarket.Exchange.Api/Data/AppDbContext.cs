using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace StockMarket.Exchange.Api.Data
{
    public class AppDbContext
    {
        public readonly IMongoCollection<Models.Exchange> Exchange;

        public AppDbContext(IConfigurationSection dbConfig)
        {
            var client = new MongoClient(dbConfig.GetValue<string>("ConnectionString"));
            var database = client.GetDatabase(dbConfig.GetValue<string>("DatabaseName"));

            var exchangeCollection = database.GetCollection<Models.Exchange>("Exchanges");
            Models.ExchangeProcessor.AddConstraints(exchangeCollection);
            this.Exchange = exchangeCollection;
        }
    }
}
