using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Programs
{
    public class ZanoxProgramResponse
    {
        [JsonProperty("programItem")]
        public IList<ZanoxProgram> Programs { get; set; } = new List<ZanoxProgram>();
    }
}
