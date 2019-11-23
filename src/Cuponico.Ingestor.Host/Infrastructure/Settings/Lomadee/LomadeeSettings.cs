using System;
using Microsoft.Extensions.Configuration;

namespace Cuponico.Ingestor.Host.Infrastructure.Settings.Lomadee
{
    public class LomadeeSettings
    {
        public LomadeeSettings(IConfigurationRoot configuration)
        {
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var section = config.GetSection(nameof(Infrastructure.Http.AffiliatePrograms.Lomadee)) ?? throw new ArgumentNullException(nameof(LomadeeSettings), "Lomadee section is not defined in configuration file.");

            var httpSection = section.GetSection(nameof(Http));
            Http = new LomadeeHttpSettings(httpSection);

            var mongoSection = section.GetSection(nameof(Mongo));
            Mongo = new LomadeeMongoSettings(mongoSection);
        }

        public LomadeeHttpSettings Http { get; }
        public LomadeeMongoSettings Mongo { get; }
    }
}