using System;
using System.Collections.Generic;

namespace Harry.Performance
{
    public class TracerBuilder: ITracerBuilder
    {
        private readonly List<ICollector> lstCollector =new List<ICollector> ();

        public void AddCollector(ICollector collector)
        {
            lstCollector.Add(collector);
        }

        public ITracer Build()
        {
            return null;
        }

        public bool TryBuild(out ITracer tracer)
        {
            throw new NotImplementedException();
        }
    }
}
