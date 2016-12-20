using System;
using System.Collections.Generic;


namespace Harry.Performance
{
    public interface IMetricCollector:ICollector
    {
        void OnStart();

        void OnComplete(long elapsedMilliseconds);
    }
}
