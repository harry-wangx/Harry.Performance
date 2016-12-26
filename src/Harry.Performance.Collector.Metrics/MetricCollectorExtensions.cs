#if !NET20
using Harry.Performance.Collecter.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harry.Performance
{
    public static class MetricCollectorExtensions
    {
        public static ITracerBuilder AddMetric(this ITracerBuilder traceBuilder)
        {
            if (traceBuilder == null)
            {
                throw new ArgumentNullException(nameof(traceBuilder));
            }

            //if (actConfig == null)
            //{
            //    throw new ArgumentNullException(nameof(actConfig));
            //}

            //ZipkinCollectorConfig cfg = new ZipkinCollectorConfig();
            //actConfig(cfg);
            //cfg.Validate();
            traceBuilder.AddCollector(new MetricCollector());
            return traceBuilder;
        }
    }
}
#endif
