using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Listing.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class PriceEntity
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string TickerSymbol { get; set; } = String.Empty;

        public decimal CurrentPrice { get; set; } = 0;

        public DateTime Time { get; set; }

        public bool IsMatch(ListingEntity other) =>
            ExchangeCode == other.ExchangeCode &&
            TickerSymbol == other.TickerSymbol;

        public bool IsMatch(string exchangeCode, string ticker) =>
            ExchangeCode == exchangeCode &&
            TickerSymbol == ticker;

        public bool IsOverlap(PriceEntity other) =>
            ExchangeCode == other.ExchangeCode &&
            TickerSymbol == other.TickerSymbol &&
            Time == other.Time;

        public PriceEntity Sanitize()
        {
            return this;
        }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (CurrentPrice <= 0)
                result.Add("currentPrice", "has to be positive");

            return result;
        }
    }

    public static class PriceCollectionManager
    {
        public static void CreateIndex(IMongoCollection<PriceEntity> collection)
        {
            var indexBuilder = Builders<PriceEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(p => p.TickerSymbol).Ascending(p => p.ExchangeCode);
            var codeModel = new CreateIndexModel<PriceEntity>(codeKey);
            collection.Indexes.CreateOne(codeModel);

            var timeKey = indexBuilder.Ascending(p => p.Time);
            var timeOpts = new CreateIndexOptions { Unique = true };
            var timeModel = new CreateIndexModel<PriceEntity>(timeKey, timeOpts);
            collection.Indexes.CreateOne(timeModel);
        }
    }
}