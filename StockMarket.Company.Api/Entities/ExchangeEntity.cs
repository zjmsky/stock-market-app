using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Company.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class ExchangeEntity
    {
        public ObjectId Id { get; set; }
        public string ExchangeCode { get; set; }
    }

    public static class ExchangeCollectionManager
    {
        public static void CreateIndex(IMongoCollection<ExchangeEntity> collection)
        {
            var indexBuilder = Builders<ExchangeEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.ExchangeCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<ExchangeEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}