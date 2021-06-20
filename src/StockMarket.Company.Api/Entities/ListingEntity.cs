using System;
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
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string TickerSymbol { get; set; } = String.Empty;
        public string CompanyCode { get; set; } = String.Empty;

        public bool IsMatch(ListingEntity other) =>
            ExchangeCode == other.ExchangeCode && TickerSymbol == other.TickerSymbol;

        public bool IsMatch(string exchangeCode, string ticker) =>
            ExchangeCode == exchangeCode && TickerSymbol == ticker;
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

        public static void Seed(IMongoCollection<ListingEntity> collection, string policy)
        {
            if (policy.ToLower() != "dev")
                return;

            if (collection.Find(x => true).Any() == false)
            {
                collection.InsertOne(new ListingEntity
                {
                    ExchangeCode = "IB",
                    TickerSymbol = "TRITONV",
                    CompanyCode = "TRITONV"
                });
                collection.InsertOne(new ListingEntity
                {
                    ExchangeCode = "IN",
                    TickerSymbol = "TITAN",
                    CompanyCode = "TITAN"
                });
            }
        }
    }
}