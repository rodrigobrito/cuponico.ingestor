using System;
using Microsoft.Extensions.Configuration;

namespace Cuponico.Ingestor.Host.Infrastructure.Settings.Advertiser
{
    public class AdvertiserSettings
    {
        public AdvertiserSettings(IConfigurationRoot configuration)
        {
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var section = config.GetSection(nameof(Advertiser)) ?? throw new ArgumentNullException(nameof(Advertiser), "Advertiser section is not defined in configuration file.");

            var mongoSection = section.GetSection(nameof(Mongo));
            Mongo = new AdvertiserMongoSettings(mongoSection);
        }

        public AdvertiserMongoSettings Mongo { get; }
    }
}