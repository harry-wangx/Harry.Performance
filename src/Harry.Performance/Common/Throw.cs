using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Performance.Common
{
    static class Throw
    {
        /// <summary>
        /// 合并多个异常
        /// </summary>
        /// <param name="exceptions"></param>
        public static Exception MergeExceptions(IEnumerable<Exception> exceptions)
        {
            StringBuilder sb = new StringBuilder(1024);
            foreach (var item in exceptions)
            {
                sb.AppendLine(item.Message);
                sb.AppendLine(item.StackTrace);
                sb.AppendLine("----------------------------------------------------");
            }
            return new Exception(sb.ToString());
        }
    }
}
