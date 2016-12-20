using System;
using System.Collections.Generic;
using Newtonsoft.Json;
#if !NET20
using System.Linq;
#endif
namespace Harry.Performance.Collector.Zipkin.Models
{
    internal sealed class JsonSpan
    {
        private readonly Span span;

        [JsonProperty("traceId")]
        public string TraceId => span.TraceHeader.TraceId.ToString("x4");

        [JsonProperty("name")]
        public string Name => span.Name;

        [JsonProperty("id")]
        public string Id => span.TraceHeader.SpanId.ToString("x4");

        [JsonProperty("parentId", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentId => span.TraceHeader.ParentId?.ToString("x4");

        [JsonProperty("annotations")]
        public IEnumerable<JsonAnnotation> Annotations
        {
            get
            {
#if NET20
                var results = new List<JsonAnnotation>();
                foreach (var item in span.Annotations)
                {
                    results.Add(new JsonAnnotation(item));
                }
                return results;
#else
                return span.Annotations.Select(annotation => new JsonAnnotation(annotation));
#endif
            }
        }


        [JsonProperty("binaryAnnotations")]
        public IEnumerable<JsonBinaryAnnotation> BinaryAnnotations
        {
            get
            {
#if NET20
                var results = new List<JsonBinaryAnnotation>();
                foreach (var item in span.BinaryAnnotations)
                {
                    results.Add(new JsonBinaryAnnotation(item));
                }
                return results;
#else
                return span.BinaryAnnotations.Select(annotation => new JsonBinaryAnnotation(annotation));
#endif
            }
        }

        public JsonSpan(Span span)
        {
            if (span == null)
                throw new ArgumentNullException(nameof(span));

            this.span = span;
        }
    }
}
