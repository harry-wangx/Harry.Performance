using System;
using System.Collections.Generic;


namespace Harry.Performance
{
    public interface ITracerBuilder
    {
        /// <summary>
        /// 添加<see cref="ICollector"/>
        /// </summary>
        /// <param name="collector"></param>
        void AddCollector(ICollector collector);

        /// <summary>
        /// 创建<see cref="ITracer"/>实例
        /// </summary>
        /// <returns></returns>
        ITracer Build();

        /// <summary>
        /// 尝试创建<see cref="ITracer"/>
        /// </summary>
        /// <param name="tracer"></param>
        /// <returns></returns>
        bool TryBuild(out ITracer tracer);

        
    }
}
