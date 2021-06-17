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
        public ObjectId Id { get; set; }

        public string CompanyCode { get; set; } = String.Empty;
        public string SectorCode { get; set; } = String.Empty;
    }

    public static class CompanyCollectionManager
    {
        public static void CreateIndex(IMongoCollection<CompanyEntity> collection)
        {
            var indexBuilder =Builders<CompanyEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(l => l.CompanyCode);
            var codeOpts = new CreateIndexOptions { Unique = true};
            var codeModel = new CreateIndexModel<CompanyEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}