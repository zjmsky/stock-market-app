using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Exchange.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Exchange
    {
        public ObjectId Id { get; set; }

        public string ExchangeCode { get; set; }
        public string CountryCode { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            var exchCodeIsValid = Regex.IsMatch(ExchangeCode, @"^[a-z]{2}$");
            if (!exchCodeIsValid) result.Add("exchangeCode", "invalid exchange code");

            var countryCodeIsValid = Regex.IsMatch(CountryCode, @"^[a-z]{2}$");
            if (!countryCodeIsValid) result.Add("countryCode", "invalid country code");

            var emailExists = Address.Email != string.Empty;
            var emailIsValid = Regex.IsMatch(Address.Email, @"@.*?\.");
            if (emailExists && !emailIsValid) result.Add("email", "invalid email");

            return result;
        }
    }

    public class ExchangeConstraintBuilder
    {
        public static void Add(IMongoCollection<Exchange> collection)
        {
            var indexBuilder = Builders<Exchange>.IndexKeys;

            var codeKey = indexBuilder.Ascending(e => e.ExchangeCode);
            var codeOpts = new CreateIndexOptions() { Unique = true };
            var codeModel = new CreateIndexModel<Exchange>(codeKey, codeOpts);
            collection.Indexes.CreateOne(codeModel);
        }
    }
}
