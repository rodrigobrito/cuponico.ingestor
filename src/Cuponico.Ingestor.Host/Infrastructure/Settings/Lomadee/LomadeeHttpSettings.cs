using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Settings.Lomadee
{
    public class LomadeeHttpSettings
    {
        private readonly IConfigurationSection _section;

        public LomadeeHttpSettings(IConfigurationSection section)
        {
            _section = section ?? throw new ArgumentNullException(nameof(section));
            Initialize();
        }

        public void Initialize()
        {
            Host = _section.GetSection(nameof(Host)).Value;
            AppToken = _section.GetSection(nameof(AppToken)).Value;
            SourceId = _section.GetSection(nameof(SourceId)).Value;
            Version = _section.GetSection(nameof(Version)).Value;
        }

        public string Version { get; private set; }
        public string AppToken { get; private set; }
        public string SourceId { get; private set; }
        public string Host { get; private set; }
        public string BaseRelativeUri => $"/{Version}/{AppToken}";
        public string GetAllCouponsUri => $"{BaseRelativeUri}/coupon/_all?sourceId={SourceId}";
        public string GetAllStoresUri => $"{BaseRelativeUri}/coupon/_stores?sourceId={SourceId}";
        public string GetAllCategoriesUri => $"{BaseRelativeUri}/coupon/_categories?sourceId={SourceId}";
        public JsonSerializerSettings JsonSettings => new JsonSerializerSettings
        {
            Culture = new CultureInfo("pt-BR"),
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.DateTime
        };
    }
}
