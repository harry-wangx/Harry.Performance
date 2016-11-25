using System;
using System.Collections.Generic;


namespace Harry.Performance
{
    public interface ITracer : IDisposable
    {
        bool IsTraceOn { get; }

        void Collect(params ISpan[] spans);
    }
}
