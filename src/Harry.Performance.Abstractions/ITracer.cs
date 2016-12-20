using System;
using System.Collections.Generic;


namespace Harry.Performance
{
    public interface ITracer 
    {
        void Collect(params Span[] spans);

        void Start();

        void Complete();
    }
}
