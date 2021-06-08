using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Company.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class ListingEntity
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string TickerSymbol { get; set; } = String.Empty;
        public string CompanyCode { get; set; } = String.Empty;

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (ExchangeCode.Length == 0)
                result.Add("exchangeCode", "required");

            if (TickerSymbol.Length == 0)
                result.Add("tickerSymbol", "required");

            if (CompanyCode.Length == 0)
                result.Add("companyCode", "required");

            return result;
        }

        public bool IsSimilar(ListingEntity other)
        {
            return ExchangeCode == other.ExchangeCode && TickerSymbol == other.TickerSymbol;
        }

        public bool IsSimilar(string exchangeCode, string ticker)
        {
            return ExchangeCode == exchangeCode && TickerSymbol == ticker;
        }
    }

    public static class ListingCollectionManager
    {
        public static void CreateIndex(IMongoCollection<ListingEntity> collection)
        {
            var indexBuilder = Builders<ListingEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.TickerSymbol).Ascending(e => e.ExchangeCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<ListingEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);

            var companyKey = indexBuilder.Ascending(e => e.CompanyCode);
            var companyModel = new CreateIndexModel<ListingEntity>(companyKey);
            collection.Indexes.CreateOne(companyModel);
        }
    }
}