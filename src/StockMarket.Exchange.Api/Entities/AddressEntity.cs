using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Exchange.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class AddressEntity
    {
        public string Line1 { get; set; } = String.Empty;
        public string Line2 { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
        public string State { get; set; } = String.Empty;
        public string Country { get; set; } = String.Empty;
        public string PhoneNumber { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;

        public AddressEntity Sanitize()
        {
            return this;
        }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            if (Email.Length == 0)
                Expression.Empty();
            else if (Regex.IsMatch(Email, @"@.*?\.") == false)
                result.Add("email", "invalid");

            return result;
        }
    }
}