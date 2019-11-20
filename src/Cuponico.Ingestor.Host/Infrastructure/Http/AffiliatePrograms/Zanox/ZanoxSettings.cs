using System;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Zanox;
using Microsoft.Extensions.Configuration;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox
{
    public class ZanoxSettings
    {
        public ZanoxSettings(IConfigurationRoot configuration)
        {
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var section = config.GetSection(nameof(Zanox)) ?? throw new ArgumentNullException(nameof(ZanoxSettings), "Zanox section is not defined in configuration file.");

            var httpSection = section.GetSection(nameof(Http));
            Http = new ZanoxHttpSettings(httpSection);

            var mongoSection = section.GetSection(nameof(Mongo));
            Mongo = new ZanoxMongoSettings(mongoSection);
        }

        public ZanoxHttpSettings Http { get; }
        public ZanoxMongoSettings Mongo { get; }
    }
}