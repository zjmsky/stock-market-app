using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Exchange.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class ListingEntity
    {
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; }
        public string TickerSymbol { get; set; }
    }

    public static class ListingCollectionManager
    {
        public static void CreateIndex(IMongoCollection<ListingEntity> collection)
        {
            var indexBuilder =Builders<ListingEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(l => l.TickerSymbol).Ascending(l => l.ExchangeCode);
            var codeOpts = new CreateIndexOptions { Unique = true};
            var codeModel = new CreateIndexModel<ListingEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}