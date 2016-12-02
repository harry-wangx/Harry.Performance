using System;
using System.Collections.Generic;

namespace Harry.Performance
{
    public class PerformanceConfig: IPerformanceConfig
    {
        /// <summary>
        /// 是否启用性能跟踪
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled { get;  set; }

        private double _sampleRate = 0;
        /// <summary>
        /// 采样率
        /// </summary>
        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException("SampleRate must range from 0 to 1.");
                }
                _sampleRate = value;
            }
        }


    }
}
