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
    public sealed class Tracer : ITracer, IDisposable
    {
        /// <summary>
        /// 当前追踪器状态
        /// </summary>
        public enum State { Waiting,Started,Stoped }

        private static Random random = new Random();
        
        private readonly List<ISpanCollector> lstSpanCollector;
        private readonly List<IMetricCollector> lstMetrictCollector;
        private readonly TraceHeader? traceHeader;

        private System.Diagnostics.Stopwatch stopwatch;
        private bool useStopwatch;
        private State state = State.Waiting;


        internal Tracer(IPerformanceConfig config, ICollector[] collectors, TraceHeader? traceHeader, bool useStopwatch = false)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            if (collectors != null && collectors.Length > 0)
            {
                ISpanCollector spanCollector = null;
                IMetricCollector metricCollector = null;
                foreach (var item in collectors)
                {
                    if ((spanCollector = item as ISpanCollector) != null)
                    {
                        if (lstSpanCollector == null)
                        {
                            lstSpanCollector = new List<ISpanCollector>();
                        }
                        lstSpanCollector.Add(spanCollector);
                    }
                    else if ((metricCollector = item as IMetricCollector) != null)
                    {
                        if (lstMetrictCollector == null)
                        {
                            lstMetrictCollector = new List<IMetricCollector>();
                        }
                        lstMetrictCollector.Add(metricCollector);
                    }
                }
            }

            this.traceHeader = traceHeader;
            this.useStopwatch = useStopwatch;

        }

        //public static Tracer TryCreate(IPerformanceConfig config, Func<bool> funcBypass = null, Func<ISpanCollector[]> funcCollectors = null, Func<TraceHeader?> funcTraceHeader = null)
        //{
        //    if (config == null) throw new ArgumentNullException(nameof(config));

        //    //配置未开启,直接返回
        //    if (!config.IsEnabled || (funcBypass != null && funcBypass()))
        //    {
        //        return null;
        //    }

        //    //获取Collector
        //    ISpanCollector[] aryCollector = null;
        //    if (funcCollectors != null)
        //    {
        //        aryCollector = funcCollectors();
        //    }
        //    else
        //    {
        //        //如果未单独提供局部Collector,则去全局查找
        //        aryCollector = CollectorManager.GetCollectors();
        //    }

        //    //之所以要在这里判断一次collector数组元素数量,是因为如果没有collector,
        //    //就没有继续追踪的必要,也就不必再获取TraceHeader
        //    if (aryCollector == null || aryCollector.Length <= 0)
        //    {
        //        return null;
        //    }

        //    TraceHeader? traceHeader = null;
        //    if (funcTraceHeader != null)
        //    {
        //        traceHeader = funcTraceHeader();
        //    }

        //    if (traceHeader == null && random.NextDouble() > config.SampleRate)
        //        return null;

        //    return new Tracer(config, aryCollector, traceHeader);
        //}


        public Span CreateSpan(EndPoint endpoint, string name = null)
        {
            var traceId = traceHeader != null
                ? traceHeader.Value.Child((ulong)random.Next())
                : new TraceHeader(traceId: (ulong)random.Next(), spanId: (ulong)random.Next());

            return new Span(traceId, endpoint, name);
        }


        private  void OnStart()
        {
            List<Exception> exceptions = null;

            foreach (IMetricCollector item in this.lstMetrictCollector)
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
                    message: "An error occurred while called method 'OnStart'", innerExceptions: exceptions);
#endif
            }

        }

        private  void OnComplete(long elapsedMilliseconds)
        {
            List<Exception> exceptions = null;

            foreach (IMetricCollector item in this.lstMetrictCollector)
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
                    message: "An error occurred while called method 'OnComplete'.", innerExceptions: exceptions);
#endif
            }
        }

        #region 接口实现
        public void Collect(params Span[] spans)
        {
            if (spans == null || spans.Length <= 0)
                return;

            List<Exception> exceptions = null;

            foreach (ISpanCollector item in this.lstSpanCollector)
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
        public void Start()
        {
            if (this.state != State.Waiting)
            {
                return;
            }

            if (useStopwatch)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
          
            this.state = State.Started;

            OnStart();
        }

        public void Complete()
        {
            if (this.state != State.Started)
            {
                return;
            }

            long elapsedMilliseconds = 0;
            if (useStopwatch)
            {
                stopwatch.Stop();
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            }
            this.state = State.Stoped;

            OnComplete(elapsedMilliseconds);
        }

        public void Dispose()
        {
            if (this.state == State.Started)
            {
                Complete();
            }

        }
        #endregion
    }
}
