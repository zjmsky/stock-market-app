using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Company.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class CompanyEntity
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public string CompanyCode { get; set; } = String.Empty;
        public string SectorCode { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        public string Turnover { get; set; } = String.Empty;
        public string CEO { get; set; } = String.Empty;
        public List<string> Directors { get; set; } = new List<string>();

        public CompanyEntity Sanitize()
        {
            SectorCode = SectorCode.ToUpper();

            return this;
        }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (CompanyCode.Length == 0)
                result.Add("companyCode", "required");

            if (Regex.IsMatch(SectorCode, @"^[A-Z]{2}$") == false)
                result.Add("sectorCode", "invalid");

            if (Name.Length == 0)
                result.Add("name", "required");
            else if (Name.Length > 50)
                result.Add("name", "too long");

            if (Description.Length > 256)
                result.Add("description", "too long");

            if (Regex.IsMatch(Turnover, @"^\d{1,3}[KMBT]$") == false)
                result.Add("turnover", "invalid");

            if (CEO.Length > 50)
                result.Add("ceo", "too long");

            if (Directors.Count > 10)
                result.Add("directors", "too many");
            if (Directors.Select(d => d.Length > 50).Count() > 0)
                result.Add("directors", "too long");

            return result;
        }
    }

    public static class CompanyCollectionManager
    {
        public static void CreateIndex(IMongoCollection<CompanyEntity> collection)
        {
            var indexBuilder = Builders<CompanyEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.CompanyCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<CompanyEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}