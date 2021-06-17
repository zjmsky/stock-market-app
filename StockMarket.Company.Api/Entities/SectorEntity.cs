using System;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Company.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class SectorEntity
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }
        
        public string SectorCode { get; set; } = String.Empty;
    }

    public static class SectorCollectionManager
    {
        public static void CreateIndex(IMongoCollection<SectorEntity> collection)
        {
            var indexBuilder = Builders<SectorEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.SectorCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<SectorEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}