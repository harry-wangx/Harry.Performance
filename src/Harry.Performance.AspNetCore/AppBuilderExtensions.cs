using Harry.Performance;
using Harry.Performance.AspNetCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class AppBuilderExtensions
    {
        public static void UseTracing(this IApplicationBuilder app, AspNetCorePerformanceConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            app.UseMiddleware<TracingMiddleware>(config);
        }
    }
}
