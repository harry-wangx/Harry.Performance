using System;
using System.Collections.Generic;


namespace Harry.Performance
{
    public interface ITracer : IDisposable
    {
        void Collect(params Span[] spans);
    }
}
