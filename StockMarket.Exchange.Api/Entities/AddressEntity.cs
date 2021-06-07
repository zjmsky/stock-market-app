using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Exchange.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class AddressEntity
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public void Sanitize()
        {
            Line1 = Line1.Trim();
            Line2 = Line2.Trim();
            State = State.Trim();
            Country = Country.Trim();
            PhoneNumber = PhoneNumber.Trim();
            Email = Email.Trim();
        }

        public Dictionary<string, string> Validate()
        {
            var result = new Dictionary<string, string>();

            var emailExists = Email != string.Empty;
            var emailIsValid = Regex.IsMatch(Email, @"@.*?\.");
            if (emailExists && !emailIsValid) result.Add("email", "invalid email");

            return result;
        }
    }
}