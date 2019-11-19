using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox
{
    public class ZanoxHttpSettings
    {
        private readonly IConfigurationSection _section;

        public ZanoxHttpSettings(IConfigurationSection section)
        {
            _section = section ?? throw new ArgumentNullException(nameof(section));
            Initialize();
        }

        public void Initialize()
        {
            BaseUrl = _section.GetSection(nameof(BaseUrl)).Value;
            SecretKey = _section.GetSection(nameof(SecretKey)).Value;
            ConnectId = _section.GetSection(nameof(ConnectId)).Value;
        }

        public string BaseUrl { get; private set; }
        public string ConnectId { get; private set; }
        public string SecretKey { get; private set; }
        public string Region { get; } = "BR";
        public string GetAllCouponsUri => $"{BaseUrl}/incentives?connectId={ConnectId}&region={Region}&incentiveType=coupons";
        public string GetStoresUri => $"{BaseUrl}/admedia?connectId={ConnectId}&region={Region}";
        public string GetProgramUri(string programId) => $"{BaseUrl}/programs/program/{programId}?connectId={ConnectId}";
        public JsonSerializerSettings JsonSettings => new JsonSerializerSettings
        {
            Culture = new CultureInfo("en-US"),
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.DateTime
        };
    }
}