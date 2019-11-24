using Elevar.Infrastructure.MongoDb;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace Cuponico.Ingestor.Host.Infrastructure.Settings.Log
{
    public class LogMongoSettings
    {
        private readonly IConfigurationSection _section;

        public LogMongoSettings(IConfigurationSection section)
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