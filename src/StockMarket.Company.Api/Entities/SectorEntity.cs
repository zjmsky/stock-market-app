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

        public static void Seed(IMongoCollection<SectorEntity> collection, string policy)
        {
            if (policy.ToLower() != "dev")
                return;

            if (collection.Find(x => true).Any() == false)
            {
                collection.InsertOne(new SectorEntity { SectorCode = "AUTO" });
                collection.InsertOne(new SectorEntity { SectorCode = "GEMS" });
            }
        }
    }
}