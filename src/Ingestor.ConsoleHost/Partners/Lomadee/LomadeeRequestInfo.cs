using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee
{
    internal class LomadeeRequestInfo
    {
        [JsonProperty("status")]
        internal string Status { get; set; }

        [JsonProperty("message")]
        internal string Message { get; set; }

        [JsonProperty("generatedDate")]
        internal object GeneratedDate { get; set; }
    }
}
