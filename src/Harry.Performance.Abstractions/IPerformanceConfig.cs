using System;
using System.Collections.Generic;


namespace Harry.Performance
{
    public interface IPerformanceConfig
    {
        bool IsEnabled { get; set; }

        double SampleRate { get; set; }
    }
}
