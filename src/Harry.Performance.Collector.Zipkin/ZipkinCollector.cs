using System;
#if NET20 || NET35
using Harry.Collections.Concurrent;
#else
using System.Collections.Concurrent;
#endif




namespace Harry.Performance.Collecter.Zipkin
{
    public sealed class ZipkinCollector : ISpanCollector
    {
        private static BlockingCollection<Span> spanQueue;
        private readonly SpanProcessor spanProcessor;


        public ZipkinCollector(ZipkinCollectorConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            config.Validate();

            var uri = config.ServerAddress;

            if (uri.PathAndQuery == "/")
                uri = new Uri(uri, "api/v1/spans");

            if (spanQueue == null)
            {
                spanQueue = new BlockingCollection<Span>(config.MaxQueueSize);
            }

            spanProcessor = new SpanProcessor(uri, spanQueue, config.MaxProcessorBatchSize);
        }


        public void Collect(params Span[] spans)
        {
            if (spans != null && spans.Length > 0)
            {
                foreach (var item in spans)
                {
                    spanQueue.Add(item);
                }
            }
        }

    }
}
