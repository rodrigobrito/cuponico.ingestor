﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Media
{
    public class ZanoxTrackinsLinks
    {
        [JsonProperty("trackingLink")]
        public IList<ZanoxTrackingLink> Links { get; set; } = new List<ZanoxTrackingLink>();
        public Uri StoreUrl => Links.FirstOrDefault()?.Uri;
        public Uri ImageUrl => Links.FirstOrDefault()?.ImageUri;
    }
}