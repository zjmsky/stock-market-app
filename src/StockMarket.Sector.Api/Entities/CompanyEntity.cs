// TODO
using System;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Sector.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class CompanyEntity
    {
        [JsonIgnore]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string CompanyCode { get; set; } = String.Empty;
        public string SectorCode { get; set; } = String.Empty;
    }

    public static class CompanyCollectionManager
    {
        public static void CreateIndex(IMongoCollection<CompanyEntity> collection)
        {
            var indexBuilder = Builders<CompanyEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(l => l.CompanyCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<CompanyEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }

        public static void Seed(IMongoCollection<CompanyEntity> collection, string policy)
        {
            if (policy.ToLower() != "dev")
                return;

            if (collection.Find(x => true).Any() == false)
            {
                collection.InsertOne(new CompanyEntity
                {
                    CompanyCode = "TRITONV",
                    SectorCode = "AUTO"
                });
                collection.InsertOne(new CompanyEntity
                {
                    CompanyCode = "TITAN",
                    SectorCode = "GEMS"
                });
            }
        }
    }
}