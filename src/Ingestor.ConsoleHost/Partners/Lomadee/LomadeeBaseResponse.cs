using Ingestor.ConsoleHost.Partners.Lomadee.Http;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee
{
    public abstract class LomadeeBaseResponse
    {
        [JsonProperty("requestInfo")]
        internal LomadeeRequestInfo RequestInfo { get; set; }
    }
}
