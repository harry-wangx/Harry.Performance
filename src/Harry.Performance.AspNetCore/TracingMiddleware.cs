using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Harry.Performance.AspNetCore
{
    public sealed class TracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AspNetCorePerformanceConfig config;

        public TracingMiddleware(RequestDelegate next, AspNetCorePerformanceConfig cfg)
        {
            _next = next;
            config = cfg;
        }

        public async Task Invoke(HttpContext context)
        {
            if (config == null
                || !config.IsEnabled)
            {
                //直接调用下一个中间件
                await _next(context);
                return;
            }
            else
            {
                //使用性能追踪
                var tracker = Tracer.TryCreate(this.config,
                    funcBypass: () =>
                    {
                        var fun = this.config.Bypass;
                        return fun != null && this.config.Bypass(context);
                    },
                    //funcCollectors: () => new ISpanCollector[] { new Collector.DebugCollector() },
                    funcTraceHeader: () =>
                    {
                        ulong traceId = 0;
                        ulong spanId = 0;
                        ulong parentSpanId = 0;
                        if (ulong.TryParse(context.Request.Headers[TraceHeader.TraceIdHttpHeaderName], out traceId)
                            && ulong.TryParse(context.Request.Headers[TraceHeader.SpanIdHttpHeaderName], out spanId)
                            && ulong.TryParse(context.Request.Headers[TraceHeader.ParentIdHttpHeaderName], out parentSpanId)
                        )
                        {
                            return new TraceHeader(traceId, spanId, parentSpanId);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    );
                if (tracker == null)
                {
                    //不进行追踪
                    await _next(context);
                    return;
                }
                else
                {
                    using (tracker)
                    {

                        EndPoint endpoint = new EndPoint()
                        {
                            IPAddress = context.Connection.LocalIpAddress,
                            Port = context.Request.Host.Port.HasValue ? context.Request.Host.Port.Value : 0,
                            ServiceName = context.Request.Host.Host,
                        };
                        string name = context.Request.Method;

                        Span span = tracker.CreateSpan(endpoint, name);
                        span.Record(Annotations.ServerReceive(DateTime.UtcNow));

                        await _next(context);

                        span.Record(Annotations.ServerSend(DateTime.UtcNow));
                        tracker.Collect(span);
                    }

                }
            }

        }
    }


}
