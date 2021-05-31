using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Company.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class Listing
    {
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; }
        public string TickerSymbol { get; set; }

        public ObjectId CompanyId { get; set; }
    }

    public static class ListingConstraintBuilder
    {
        public static void Add(IMongoCollection<Listing> collection)
        {
            var indexBuilder = Builders<Listing>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.TickerSymbol).Ascending(e => e.ExchangeCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<Listing>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}