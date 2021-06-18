using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Listing.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class IpoEntity
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string TickerSymbol { get; set; } = String.Empty;

        public decimal PricePerShare { get; set; } = 0;
        public uint TotalShares { get; set; } = 0;

        public DateTime OpenTime { get; set; }

        public bool IsMatch(IpoEntity other) =>
            ExchangeCode == other.ExchangeCode &&
            TickerSymbol == other.TickerSymbol;

        public bool IsMatch(ListingEntity other) =>
            ExchangeCode == other.ExchangeCode &&
            TickerSymbol == other.TickerSymbol;

        public bool IsMatch(string exchangeCode, string ticker) =>
            ExchangeCode == exchangeCode &&
            TickerSymbol == ticker;

        public IpoEntity Sanitize()
        {
            return this;
        }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (PricePerShare <= 0)
                result.Add("pricePerShare", "has to be positive");

            if (TotalShares == 0)
                result.Add("totalShares", "cannot be 0");

            return result;
        }
    }

    public static class IpoCollectionManager
    {
        public static void CreateIndex(IMongoCollection<IpoEntity> collection)
        {
            var indexBuilder = Builders<IpoEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.TickerSymbol).Ascending(e => e.ExchangeCode);
            var codeOpts = new CreateIndexOptions { Unique = true };
            var codeModel = new CreateIndexModel<IpoEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}