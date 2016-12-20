using System;
using Newtonsoft.Json;

namespace Harry.Performance.Collector.Zipkin.Models
{
    internal sealed class JsonAnnotation
    {
        private readonly Annotation annotation;

        [JsonProperty("endpoint")]
        public JsonEndpoint Endpoint { get; set; }

        [JsonProperty("value")]
        public string Value => annotation.Value;

        [JsonProperty("timestamp")]
        public long Timestamp =>Harry.Common.Utils.GetTimeStamp(annotation.Timestamp) * 1000;

        public JsonAnnotation(Annotation annotation)
        {
            this.annotation = annotation;
            this.Endpoint= new JsonEndpoint(annotation.Endpoint);
        }
    }
}