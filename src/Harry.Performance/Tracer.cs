using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
#if ASYNC
using System.Threading.Tasks;
#endif

namespace Harry.Performance
{
    public class Tracer : ITracer
    {
        private static Random random = new Random();


        private readonly System.Diagnostics.Stopwatch stopwatch;
        private readonly List<ISpanCollector> lstCollector;
        private readonly TraceHeader? traceHeader;
        private bool useStopwatch;


        private Tracer(IPerformanceConfig config, ISpanCollector[] collectors, TraceHeader? traceHeader, bool useStopwatch = false)
        {
            this.lstCollector = new List<ISpanCollector>();
            this.lstCollector.AddRange(collectors);
            this.traceHeader = traceHeader;

            onStart();

            this.useStopwatch = useStopwatch;
            if (useStopwatch)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
        }

        public static Tracer TryCreate(IPerformanceConfig config, Func<bool> funcBypass = null, Func<ISpanCollector[]> funcCollectors = null, Func<TraceHeader?> funcTraceHeader = null)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            //配置未开启,直接返回
            if (!config.IsEnabled || (funcBypass != null && funcBypass()))
            {
                return null;
            }

            //获取Collector
            ISpanCollector[] aryCollector = null;
            if (funcCollectors != null)
            {
                aryCollector = funcCollectors();
            }
            else
            {
                //如果未单独提供局部Collector,则去全局查找
                aryCollector = CollectorManager.GetCollectors();
            }

            //之所以要在这里判断一次collector数组元素数量,是因为如果没有collector,
            //就没有继续追踪的必要,也就不必再获取TraceHeader
            if (aryCollector == null || aryCollector.Length <= 0)
            {
                return null;
            }

            TraceHeader? traceHeader = null;
            if (funcTraceHeader != null)
            {
                traceHeader = funcTraceHeader();
            }

            if (traceHeader == null && random.NextDouble() > config.SampleRate)
                return null;

            return new Tracer(config, aryCollector, traceHeader);
        }


        public Span CreateSpan(EndPoint endpoint, string name = null)
        {
            var traceId = traceHeader != null
                ? traceHeader.Value.Child((ulong)random.Next())
                : new TraceHeader(traceId: (ulong)random.Next(), spanId: (ulong)random.Next());

            return new Span(traceId, endpoint, name);
        }


        public void Collect(params Span[] spans)
        {
            if (spans == null || spans.Length <= 0)
                return;

            List<Exception> exceptions = null;

            foreach (ISpanCollector item in this.lstCollector)
            {
                try
                {
                    item.Collect(spans);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }
                    exceptions.Add(ex);
                }
            }
            if (exceptions != null && exceptions.Count > 0)
            {
#if NET20 || NET35
                throw Common.Throw.MergeExceptions(exceptions);
#else

                throw new AggregateException(
                    message: "An error occurred while writing to collector(s).", innerExceptions: exceptions);
#endif
            }

        }


        public void Dispose()
        {
            long elapsedMilliseconds = 0;
            if (useStopwatch)
            {
                stopwatch.Stop();
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            }

            List<Exception> exceptions = null;

            foreach (ISpanCollector item in this.lstCollector)
            {
                try
                {
                    item.OnComplete(elapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }
                    exceptions.Add(ex);
                }
            }
            if (exceptions != null && exceptions.Count > 0)
            {
#if NET20 || NET35
                throw Common.Throw.MergeExceptions(exceptions);
#else

                throw new AggregateException(
                    message: "An error occurred while writing to collector(s).", innerExceptions: exceptions);
#endif
            }

        }


        private void onStart()
        {
            List<Exception> exceptions = null;

            foreach (ISpanCollector item in this.lstCollector)
            {
                try
                {
                    item.OnStart();
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }
                    exceptions.Add(ex);
                }
            }
            if (exceptions != null && exceptions.Count > 0)
            {
#if NET20 || NET35
                throw Common.Throw.MergeExceptions(exceptions);
#else

                throw new AggregateException(
                    message: "An error occurred while writing to collector(s).", innerExceptions: exceptions);
#endif
            }

        }

    }
}
