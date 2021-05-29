using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Exchange.Api.Models
{
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
    }

    public class ExchangeProcessor
    {
        public static void Validate(Exchange document)
        {
            // required + 2 letters
            var exchCodeIsValid = Regex.IsMatch(document.ExchangeCode, @"^[a-zA-Z]{2}$");
            if (!exchCodeIsValid) throw new ValidationException("ExchangeCode");

            // required + 2 letters
            var countryCodeIsValid = Regex.IsMatch(document.CountryCode, @"^[a-zA-Z]{2}$");
            if (!countryCodeIsValid) throw new ValidationException("CountryCode");

            // optional + valid email
            var emailExists = document.Address.Email != string.Empty;
            var emailIsValid = Regex.IsMatch(document.Address.Email, @"@.*?\.");
            if (emailExists && !emailIsValid) throw new ValidationException("Email");
        }

        public static void Sanitize(Exchange document)
        {
            document.ExchangeCode = document.ExchangeCode.ToUpper();
            document.CountryCode = document.CountryCode.ToUpper();
        }

        public static async void AddConstraints(IMongoCollection<Exchange> collection)
        {
            var indexBuilder = Builders<Models.Exchange>.IndexKeys;

            var nameIndexKey = indexBuilder.Ascending(e => e.Name);
            var nameIndexOpts = new CreateIndexOptions() { Unique = true };
            var nameIndexModel = new CreateIndexModel<Models.Exchange>(nameIndexKey, nameIndexOpts);
            await collection.Indexes.CreateOneAsync(nameIndexModel);

            var codeIndexKey = indexBuilder.Ascending(e => e.ExchangeCode);
            var codeIndexOpts = new CreateIndexOptions() { Unique = true };
            var codeIndexModel = new CreateIndexModel<Models.Exchange>(codeIndexKey, codeIndexOpts);
            await collection.Indexes.CreateOneAsync(codeIndexModel);
        }
    }
}
