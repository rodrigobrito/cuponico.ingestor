using System;
using Elevar.Infrastructure.MongoDb;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser
{
    public class AdvertiserMongoSettings
    {
        private readonly IConfigurationSection _section;

        public AdvertiserMongoSettings(IConfigurationRoot config)
        {
            var section = config.GetSection(nameof(Advertiser)) ?? throw new ArgumentNullException(nameof(config), "Advertiser settings section is not defined in configuration file.");
            _section = section.GetSection("Mongo");
        }

        public string ConnectionString => _section.GetValue<string>("ConnectionString");
        public string Database => _section.GetValue<string>("Database");

        public IMongoWrapper CreateWrapper()
        {
            var client = new MongoClient(ConnectionString);
            return new MongoWrapper(client.GetDatabase(Database));
        }
    }
}
