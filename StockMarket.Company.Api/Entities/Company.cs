using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockMarket.Company.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class StockListing
    {
        public string ExchangeCode { get; set; }
        public string TickerSymbol { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Company
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Turnover { get; set; }
        public string CEO { get; set; }
        public List<string> Directors { get; set; }

        public string SectorCode { get; set; }

        public List<StockListing> Listings { get; set; }
    }

    public static class CompanyConstraintBuilder
    {
        public static void Add(IMongoCollection<Company> companyCollection)
        {
            var indexBuilder = Builders<Company>.IndexKeys;

            var nameKey = indexBuilder.Ascending(e => e.Name);
            var nameOpts = new CreateIndexOptions { Unique = true };
            var nameModel = new CreateIndexModel<Company>(nameKey, nameOpts);
            companyCollection.Indexes.CreateOne(nameModel);
        }
    }
}