using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Http
{
    internal abstract class LomadeeBaseResponse
    {
        [JsonProperty("requestInfo")]
        internal LomadeeRequestInfo RequestInfo { get; set; }
    }
}
