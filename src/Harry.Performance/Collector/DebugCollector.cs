using System;
using System.Collections.Generic;
using System.IO;
#if !NET20 && !NET35
using System.Threading.Tasks;
#endif

namespace Harry.Performance.Collector
{
    public sealed class DebugCollector : ISpanCollector
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


#if !NET20 && !NET35

        public
#if ASYNC
                        async
#endif

            Task CollectAsync(params Span[] spans)
        {
            foreach (var span in spans)
                _writer.WriteLine(span.ToString());

            _writer.Flush();
            //throw new Exception();
        }

#endif
    }
}
