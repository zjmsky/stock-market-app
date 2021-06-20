using System;
using System.Linq.Expressions;
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
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string TickerSymbol { get; set; } = String.Empty;

        public decimal CurrentPrice { get; set; } = 0;

        public DateTime Time { get; set; }

        public static Expression<Func<PriceEntity, bool>> IsMatch(string exchange, string ticker)
        {
            return p => p.TickerSymbol == ticker && p.ExchangeCode == exchange;
        }

        public static Expression<Func<PriceEntity, bool>> IsOverlap(PriceEntity other)
        {
            return p => 
                p.TickerSymbol == other.TickerSymbol &&
                p.ExchangeCode == other.ExchangeCode &&
                p.Time == other.Time;
        }

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
            var timeModel = new CreateIndexModel<PriceEntity>(timeKey);
            collection.Indexes.CreateOne(timeModel);
        }

        public static void Seed(IMongoCollection<PriceEntity> collection, string policy)
        {
            if (policy.ToLower() != "dev")
                return;

            if (collection.Find(x => true).Any() == false)
            {
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 200.47M, Time = new DateTime(2021, 6, 2) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 204.88M, Time = new DateTime(2021, 6, 3) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 190.63M, Time = new DateTime(2021, 6, 4) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 203.38M, Time = new DateTime(2021, 6, 5) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 209.45M, Time = new DateTime(2021, 6, 6) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 233.46M, Time = new DateTime(2021, 6, 7) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 222.68M, Time = new DateTime(2021, 6, 8) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 224.62M, Time = new DateTime(2021, 6, 9) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 210.19M, Time = new DateTime(2021, 6, 10) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 208.03M, Time = new DateTime(2021, 6, 11) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IS", TickerSymbol = "TITAN", CurrentPrice = 213.77M, Time = new DateTime(2021, 6, 12) });
            
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 160.85M, Time = new DateTime(2021, 6, 2) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 163.06M, Time = new DateTime(2021, 6, 3) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 166.67M, Time = new DateTime(2021, 6, 4) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 151.68M, Time = new DateTime(2021, 6, 5) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 145.51M, Time = new DateTime(2021, 6, 6) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 170.86M, Time = new DateTime(2021, 6, 7) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 179.98M, Time = new DateTime(2021, 6, 8) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 181.10M, Time = new DateTime(2021, 6, 9) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 195.29M, Time = new DateTime(2021, 6, 10) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 190.15M, Time = new DateTime(2021, 6, 11) });
                collection.InsertOne(new PriceEntity { ExchangeCode = "IB", TickerSymbol = "TRITONV", CurrentPrice = 200.88M, Time = new DateTime(2021, 6, 12) });
            }
        }
    }
}