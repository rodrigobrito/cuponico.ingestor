using System;
using Elevar.Infrastructure.MongoDb;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Ingestor.ConsoleHost.Partners.Lomadee
{
    public class LomadeeMongoSettings
    {
        private readonly IConfigurationSection _section;

        public LomadeeMongoSettings(IConfigurationSection section)
        {
            _section = section ?? throw new ArgumentNullException(nameof(section));
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