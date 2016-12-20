using System;
using Newtonsoft.Json;
using System.Net;

namespace Harry.Performance.Collector.Zipkin.Models
{
    internal sealed class JsonEndpoint
    {
        private readonly EndPoint endpoint;

        [JsonProperty("ipv4")]
        public string IPv4 => endpoint.IPAddress.ToString();

        [JsonProperty("port")]
        public int Port => endpoint.Port;

        [JsonProperty("serviceName")]
        public string ServiceName => endpoint.ServiceName;

        public JsonEndpoint(EndPoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            this.endpoint = endpoint;
        }
    }
}