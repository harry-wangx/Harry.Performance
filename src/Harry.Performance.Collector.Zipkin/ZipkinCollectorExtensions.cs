#if !NET20
using Harry.Performance.Collecter.Zipkin;
using System;

namespace Harry.Performance
{
    public static class ZipkinCollectorExtensions
    {
        public static ITracerBuilder AddZipkin(this ITracerBuilder traceBuilder, Action<ZipkinCollectorConfig> actConfig)
        {
            if (traceBuilder == null)
            {
                throw new ArgumentNullException(nameof(traceBuilder));
            }

            if (actConfig == null)
            {
                throw new ArgumentNullException(nameof(actConfig));
            }

            ZipkinCollectorConfig cfg = new ZipkinCollectorConfig();
            actConfig(cfg);
            cfg.Validate();
            traceBuilder.AddCollector(new ZipkinCollector(cfg));
            return traceBuilder;
        }
    }
}

#endif