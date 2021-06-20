using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Exchange.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class ListingEntity
    {
        [JsonIgnore]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string TickerSymbol { get; set; } = String.Empty;

        public static Expression<Func<ListingEntity, bool>> IsMatch(string exchange, string ticker)
        {
            return l => l.TickerSymbol == ticker && l.ExchangeCode == exchange;
        }
    }

    public static class ListingCollectionManager
    {
        public static void CreateIndex(IMongoCollection<ListingEntity> collection)
        {
            var indexBuilder = Builders<ListingEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(l => l.TickerSymbol).Ascending(l => l.ExchangeCode);
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
                    TickerSymbol = "TITAN"
                });
                collection.InsertOne(new ListingEntity
                {
                    ExchangeCode = "IB",
                    TickerSymbol = "TRITONV"
                });
            }
        }
    }
}