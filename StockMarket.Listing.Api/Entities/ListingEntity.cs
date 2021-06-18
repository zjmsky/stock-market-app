using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Listing.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class ListingEntity
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string TickerSymbol { get; set; } = String.Empty;
        public string CompanyCode { get; set; } = String.Empty;

        public bool IsMatch(ListingEntity other) =>
            ExchangeCode == other.ExchangeCode &&
            TickerSymbol == other.TickerSymbol;

        public bool IsMatch(IpoEntity other) =>
            ExchangeCode == other.ExchangeCode &&
            TickerSymbol == other.TickerSymbol;

        public bool IsMatch(PriceEntity other) =>
            ExchangeCode == other.ExchangeCode &&
            TickerSymbol == other.TickerSymbol;

        public bool IsMatch(string exchangeCode, string ticker) =>
            ExchangeCode == exchangeCode &&
            TickerSymbol == ticker;

        public ListingEntity Sanitize()
        {
            return this;
        }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (Regex.IsMatch(ExchangeCode, @"^[A-Z]{2}$") == false)
                result.Add("exchangeCode", "invalid");

            return result;
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
        }
    }
}