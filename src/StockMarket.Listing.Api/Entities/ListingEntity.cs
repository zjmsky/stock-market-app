using System;
using System.Linq.Expressions;
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
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string TickerSymbol { get; set; } = String.Empty;
        public string CompanyCode { get; set; } = String.Empty;

        public static Expression<Func<ListingEntity, bool>> IsMatch(string exchange, string ticker)
        {
            return l => l.TickerSymbol == ticker && l.ExchangeCode == exchange;
        }
        
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

        public static void Seed(IMongoCollection<ListingEntity> collection, string policy)
        {
            if (policy.ToLower() != "dev")
                return;

            if (collection.Find(x => true).Any() == false)
            {
                collection.InsertOne(new ListingEntity
                {
                    ExchangeCode = "IS",
                    TickerSymbol = "TITAN",
                    CompanyCode = "TITAN",
                });
                collection.InsertOne(new ListingEntity
                {
                    ExchangeCode = "IB",
                    TickerSymbol = "TRITONV",
                    CompanyCode = "TRITONV",
                });
            }
        }
    }
}