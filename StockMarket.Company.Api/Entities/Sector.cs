using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Company.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class Sector
    {
        public ObjectId Id { get; set; }
        public string SectorCode { get; set; }
    }

    public static class SectorConstraintBuilder
    {
        public static void Add(IMongoCollection<Sector> collection)
        {
            var indexBuilder = Builders<Sector>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.SectorCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<Sector>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}