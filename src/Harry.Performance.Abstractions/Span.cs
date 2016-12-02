using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Performance
{
    /// <summary>
    /// A set of <see cref="Annotation"/> and <see cref="BinaryAnnotation"/> elements that correspond to a particular RPC. 
    /// Spans contain identifying information such as traceId, spandId, parentId, and RPC name.
    /// </summary>
    public sealed class Span 
    {
        /// <summary>
        /// Trace header containing are identifiers necessary to locate current span.
        /// </summary>
        public readonly TraceHeader TraceHeader;

        public readonly string Name;

        /// <summary>
        /// Collection of annotations recorder withing current span time frame.
        /// </summary>
        public readonly ICollection<Annotation> Annotations;

        /// <summary>
        /// Collection of binary annotations used to attach additional metadata with the span itself.
        /// </summary>
        public readonly ICollection<BinaryAnnotation> BinaryAnnotations;

        /// <summary>
        /// Endpoint, target span's service is listening on.
        /// </summary>
        public readonly EndPoint Endpoint;

        public Span(TraceHeader traceHeader, EndPoint endpoint, string name = null)
        {
            TraceHeader = traceHeader;
            Annotations = new List<Annotation>();
            BinaryAnnotations = new List<BinaryAnnotation>();
            Endpoint = endpoint;
            this.Endpoint.IPAddress = this.Endpoint.IPAddress ?? new System.Net.IPAddress(0);
            this.Endpoint.ServiceName = endpoint.ServiceName ?? "Unknown";
            Name = name ?? "Unknown";
        }

        public Span(TraceHeader traceHeader, EndPoint endpoint, ICollection<Annotation> annotations, ICollection<BinaryAnnotation> binaryAnnotations, string name)
        {
            TraceHeader = traceHeader;
            Annotations = annotations;
            BinaryAnnotations = binaryAnnotations;
            Endpoint = endpoint;
            this.Endpoint.IPAddress = this.Endpoint.IPAddress ?? new System.Net.IPAddress(0);
            this.Endpoint.ServiceName = endpoint.ServiceName ?? "Unknown";
            Name = name ?? "Unknown";
        }

        public Span(ulong traceId, ulong spanId, ulong? parentId = null)
            : this(new TraceHeader(traceId, spanId, parentId, true), new EndPoint() { IPAddress=new System.Net.IPAddress(0), Port=0 })
        {
        }

        /// <summary>
        /// Records an annotation within current span. 
        /// Also sets it's endpoint if it was not set previously.
        /// </summary>
        public void Record(Annotation annotation)
        {
            if (annotation.Endpoint == null)
            {
                annotation = annotation.WithEndpoint(Endpoint);
            }

            Annotations.Add(annotation);
        }

        /// <summary>
        /// Records a binary annotation within current span. 
        /// Also sets it's endpoint if it was not set previously.
        /// </summary>
        public void Record(BinaryAnnotation binaryAnnotation)
        {
            if (binaryAnnotation.Endpoint == null)
            {
                binaryAnnotation = binaryAnnotation.WithEndpoint(Endpoint);
            }

            BinaryAnnotations.Add(binaryAnnotation);
        }

        public override string ToString()
        {
            var sb = new StringBuilder()
                .Append("Span(service:").Append(this.Endpoint.ServiceName).Append(", name:").Append(Name)
                .Append(", trace:").Append(TraceHeader.ToString())
                .Append(", endpoint:").Append(Endpoint.ToString())
                .Append(", annotations:[");

            foreach (var annotation in Annotations)
            {
                sb.Append(annotation.ToString()).Append(' ');
            }

            if (BinaryAnnotations.Count > 0)
            {
                sb.Append("], binnaryAnnotations:[");

                foreach (var annotation in BinaryAnnotations)
                {
                    sb.Append(annotation.ToString()).Append(' ');
                }
            }

            sb.Append("])");

            return sb.ToString();
        }
    }
}
