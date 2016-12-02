using System;
using System.Collections.Generic;

namespace Harry.Performance
{
    public static class CollectorManager
    {
        private readonly static List<ISpanCollector> lstCollectors = new List<ISpanCollector>();

        public static void Register(ISpanCollector collector)
        {
            if (collector == null)
                throw new ArgumentNullException(nameof(collector));
            lstCollectors.Add(collector);
        }

        public static ISpanCollector[] GetCollectors()
        {
            return lstCollectors.ToArray();
        }
    }
}
