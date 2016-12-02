using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Harry.Performance.AspNetCore
{
    public class AspNetCorePerformanceConfig : PerformanceConfig
    {
        /// <summary>
        /// 是否跳过,不进行性能追踪
        /// </summary>
        public Predicate<HttpContext> Bypass { get; set; } = r => false;
    }
}
