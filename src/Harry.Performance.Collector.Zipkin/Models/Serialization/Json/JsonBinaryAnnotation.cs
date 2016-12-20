using System;
using Newtonsoft.Json;

namespace Harry.Performance.Collector.Zipkin.Models
{
    internal sealed class JsonBinaryAnnotation
    {
        private readonly BinaryAnnotation binaryAnnotation;

        [JsonProperty("endpoint")]
        public JsonEndpoint Endpoint { get; set; }

        [JsonProperty("key")]
        public string Key => binaryAnnotation.Key;

        [JsonProperty("value")]
        public string Value => binaryAnnotation.Value.ToString();

        public JsonBinaryAnnotation(BinaryAnnotation binaryAnnotation)
        {
            this.binaryAnnotation = binaryAnnotation;
            this.Endpoint= new JsonEndpoint(binaryAnnotation.Endpoint);
        }
    }
}