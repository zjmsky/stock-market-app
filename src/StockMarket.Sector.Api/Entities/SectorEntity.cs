using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Sector.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class SectorEntity
    {
        [JsonIgnore]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string SectorCode { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        public SectorEntity Sanitize()
        {
            SectorCode = SectorCode.Trim().ToUpper();
            Name = Name.Trim();
            Description = Description.Trim();
            return this;
        }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (Regex.IsMatch(SectorCode, @"^[A-Z]{2}$") == false)
                result.Add("sectorCode", "invalid");

            if (Name.Length == 0)
                result.Add("name", "required");
            else if (Name.Length > 30)
                result.Add("name", "too long");

            if (Description.Length > 256)
                result.Add("description", "too long");

            return result;
        }
    }

    public class SectorCollectionManager
    {
        public static void CreateIndex(IMongoCollection<SectorEntity> collection)
        {
            var indexBuilder = Builders<SectorEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.SectorCode);
            var codeOpts = new CreateIndexOptions() { Unique = true };
            var codeModel = new CreateIndexModel<SectorEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }

        public static void Seed(IMongoCollection<SectorEntity> collection, string policy)
        {
            if (policy.ToLower() != "dev")
                return;

            if (collection.Find(x => true).Any() == false)
            {
                collection.InsertOne(new SectorEntity
                {
                    SectorCode = "AUTO",
                    Name = "Auto Components",
                    Description = "This is description for Auto",
                });
                collection.InsertOne(new SectorEntity
                {
                    SectorCode = "GEMS",
                    Name = "Gems, Jewels and Watches",
                    Description = "This is description for Gems",
                });
            }
        }
    }
}