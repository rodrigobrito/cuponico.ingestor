﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Incentives
{
    public class IncentiveItems
    {
        [JsonProperty("incentiveItem")]
        public IList<IncentiveItem> Items { get; set; } = new List<IncentiveItem>();
    }
}
