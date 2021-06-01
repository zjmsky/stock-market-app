using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Company.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class Exchange
    {
        public ObjectId Id { get; set; }
        public string ExchangeCode { get; set; }
    }

    public static class ExchangeConstraintBuilder
    {
        public static void Add(IMongoCollection<Exchange> collection)
        {
            var indexBuilder = Builders<Exchange>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.ExchangeCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<Exchange>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}