using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Sector.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class Sector
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public string SectorCode { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            var sectorCodeIsValid = Regex.IsMatch(SectorCode, "@^[A-Z]{2}$");
            if (!sectorCodeIsValid) result.Add("sectorCode", "invalid sector code");

            return result;
        }
    }

    public class SectorConstraintBuilder
    {
        public static void Add(IMongoCollection<Sector> collection)
        {
            var indexBuilder = Builders<Sector>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.SectorCode);
            var codeOpts = new CreateIndexOptions() { Unique = true };
            var codeModel = new CreateIndexModel<Sector>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}