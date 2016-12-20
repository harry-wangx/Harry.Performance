using System;
using System.Collections.Generic;
using System.IO;
#if !NET20 && !NET35
using System.Threading.Tasks;
#endif

namespace Harry.Performance.Collector
{
    public sealed class DebugCollector : ISpanCollector,IMetricCollector
    {
        private readonly TextWriter _writer;

        public DebugCollector(TextWriter writer)
        {
            _writer = writer;
        }

        /// <summary>
        /// Creates a debug collector instance logging all data on the standard output.
        /// </summary>
        public DebugCollector() : this(System.Console.Out)
        {
        }

        public void Collect(params Span[] spans)
        {
            foreach (var span in spans)
                _writer.WriteLine(span.ToString());

            _writer.Flush();
        }

        public void OnStart()
        {
            _writer.WriteLine(string.Format("[0]called OnStart",DateTime.Now.ToString()));
        }

        public void OnComplete(long elapsedMilliseconds)
        {
            _writer.WriteLine(string.Format("[0]called OnComplete", DateTime.Now.ToString()));
        }
    }
}
