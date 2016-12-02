using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Harry.Performance
{
    [StructLayout(LayoutKind.Auto)]
    public struct Annotation : IEquatable<Annotation>
    {
        /// <summary>
        /// Timestamp marking the occurrence of an event.
        /// </summary>
        public readonly DateTime Timestamp;

        /// <summary>
        /// Value holding an information about the annotation. 
        /// See <see cref="AnnotationConstants"/> for some 
        /// build in Zipkin annotation values.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Service endpoint.
        /// </summary>
        public readonly EndPoint Endpoint;

        public Annotation(string value, DateTime timestamp, EndPoint endpoint)
        {
            Timestamp = timestamp;
            Value = value;
            Endpoint = endpoint;
        }

        public Annotation(string value, DateTime timestamp) : this()
        {
            Timestamp = timestamp;
            Value = value;
        }

        /// <summary>
        /// Returns a new instance of the <see cref="Annotation"/> with 
        /// <paramref name="timestamp"/> set and all other fields copied 
        /// from current instance.
        /// </summary>
        public Annotation WithTimestamp(DateTime timestamp) => new Annotation(Value, timestamp, Endpoint);

        /// <summary>
        /// Returns a new instance of the <see cref="Annotation"/> with 
        /// <paramref name="endpoint"/> set and all other fields copied 
        /// from current instance.
        /// </summary>
        public Annotation WithEndpoint(EndPoint endpoint) => new Annotation(Value, Timestamp, endpoint);

        public bool Equals(Annotation other) => other.Timestamp == Timestamp && Equals(other.Value, Value) && Equals(other.Endpoint, Endpoint);

        public override bool Equals(object obj) => obj is Annotation && Equals((Annotation)obj);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Timestamp.GetHashCode();
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Endpoint.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"Annotation({Value}, {Timestamp.ToString("O")}, {Endpoint})";
    }
}
