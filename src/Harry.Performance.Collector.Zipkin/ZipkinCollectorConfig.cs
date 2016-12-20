using System;
using System.Collections.Generic;

namespace Harry.Performance.Collecter.Zipkin
{
    public class ZipkinCollectorConfig
    {
        public ZipkinCollectorConfig()
        {
            this.ServerAddress = new Uri("http://localhost:9411/");
            MaxQueueSize = 100;
            MaxProcessorBatchSize = 20;
        }
        /// <summary>
        /// Zipkin服务器地址
        /// </summary>
        public Uri ServerAddress { get; set; }

        /// <summary>
        /// 初始化用于临时存放span的集合的最大上限值(需大于等于0)
        /// </summary>
        public int MaxQueueSize { get; set; }


        public uint MaxProcessorBatchSize { get; set; }

        /// <summary>
        /// 验证数据
        /// </summary>
        public void Validate()
        {
            if (ServerAddress == null)
            {
                throw new NullReferenceException("ServerAddress属性不能为空");
            }

            if (!ServerAddress.IsAbsoluteUri)
                throw new ArgumentException($"URI '{ServerAddress}' should be an absolute URI path to zipkin server");

            if (MaxQueueSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(MaxQueueSize));
        }
    }
}
