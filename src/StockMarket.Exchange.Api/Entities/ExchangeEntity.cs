using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Exchange.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class ExchangeEntity
    {
        [JsonIgnore]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; } = String.Empty;
        public string CountryCode { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public AddressEntity Address { get; set; } = null;

        public ExchangeEntity Sanitize()
        {
            if (Address != null) Address.Sanitize();
            return this;
        }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (Regex.IsMatch(ExchangeCode, @"^[A-Z]{2}$") == false)
                result.Add("exchangeCode", "invalid");

            if (Regex.IsMatch(CountryCode, @"^[A-Z]{2}$") == false)
                result.Add("countryCode", "invalid");

            if (Address != null)
                Address.Validate().ToList()
                    .ForEach(r => result.Add($"address.{r.Key}", r.Value));

            return result;
        }
    }

    public class ExchangeCollectionManager
    {
        public static void CreateIndex(IMongoCollection<ExchangeEntity> collection)
        {
            var indexBuilder = Builders<ExchangeEntity>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.ExchangeCode);
            var codeOpts = new CreateIndexOptions() { Unique = true };
            var codeModel = new CreateIndexModel<ExchangeEntity>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }

        public static void Seed(IMongoCollection<ExchangeEntity> collection, string policy)
        {
            if (policy.ToLower() != "dev")
                return;

            if (collection.Find(x => true).Any() == false)
            {
                collection.InsertOne(new ExchangeEntity
                {
                    ExchangeCode = "IS",
                    CountryCode = "IN",
                    Name = "National Stock Exchange",
                    Description = "This is National Stock Exchange",
                    Address = new AddressEntity
                    {
                        Line1 = "Exchange Plaza, C-1",
                        Line2 = "Bandra Kurla Complex",
                        City = "Mumbai",
                        State = "Maharashtra",
                        Country = "India",
                        PhoneNumber = "9876543210",
                        Email = "nse@test.com"
                    }
                });
                collection.InsertOne(new ExchangeEntity
                {
                    ExchangeCode = "IB",
                    CountryCode = "IN",
                    Name = "Bombay Stock Exchange",
                    Description = "This is Bombay Stock Exchange",
                    Address = new AddressEntity
                    {
                        Line1 = "Phiroze Jeejeebhoy Towers",
                        Line2 = "Dalal Street",
                        City = "Mumbai",
                        State = "Maharashtra",
                        Country = "India",
                        PhoneNumber = "8976543210",
                        Email = "bse@test.com"
                    }
                });
            }
        }
    }
}
