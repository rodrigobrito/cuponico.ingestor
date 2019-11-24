using System;
using Microsoft.Extensions.Configuration;

namespace Cuponico.Ingestor.Host.Infrastructure.Settings.Log
{
    public class LogSettings
    {
        public LogSettings(IConfigurationRoot configuration)
        {
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var section = config.GetSection(nameof(Log)) ?? throw new ArgumentNullException(nameof(Log), "Log section is not defined in configuration file.");

            var mongoSection = section.GetSection(nameof(Mongo));
            Mongo = new LogMongoSettings(mongoSection);
        }

        public LogMongoSettings Mongo { get; }
    }
}