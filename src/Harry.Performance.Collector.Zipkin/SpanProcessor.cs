using Harry.Performance.Collector.Zipkin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Harry.Common;

#if NET20 || NET35
using Harry.Collections.Concurrent;
#else
using System.Collections.Concurrent;
using System.Linq;
#endif

namespace Harry.Performance.Collecter.Zipkin
{
    public class SpanProcessor
    {
        internal const int MAX_NUMBER_OF_POLLS = 5;

        private readonly Uri uri;
        private readonly BlockingCollection<Span> spanQueue;


        private readonly ConcurrentQueue<JsonSpan> serializableSpans;
        private readonly ProcessorTaskFactory spanProcessorTaskFactory;

        private int subsequentPollCount;
        private readonly uint maxBatchSize;
        private readonly Harry.Logging.ILogger logger;

#if COREFX
        private System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
#endif

        public SpanProcessor(Uri uri, BlockingCollection<Span> spanQueue, uint maxBatchSize)
        {
            if (spanQueue == null)
            {
                throw new ArgumentNullException(nameof(spanQueue));
            }

            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            this.uri = uri;
            this.spanQueue = spanQueue;
            this.serializableSpans = new ConcurrentQueue<JsonSpan>();
            this.maxBatchSize = maxBatchSize;
            this.logger = Harry.Logging.LoggerFactory.Instance.CreateLogger("Harry.Performance.Collecter.Zipkin.SpanProcessor");
            spanProcessorTaskFactory = new ProcessorTaskFactory(logger);
        }

        /// <summary>
        /// 停止发送追踪信息
        /// </summary>
        public virtual void Stop()
        {
            spanProcessorTaskFactory.StopTask();
            LogSubmittedSpans();
        }

        /// <summary>
        /// 开始发送追踪信息
        /// </summary>
        public virtual void Start()
        {
            spanProcessorTaskFactory.CreateAndStart(LogSubmittedSpans);
        }

        /// <summary>
        /// 发送追踪信息到zipkin服务器
        /// </summary>
        internal virtual void LogSubmittedSpans()
        {
            var anyNewSpans = ProcessQueuedSpans();

            //有新的信息
            if (anyNewSpans) subsequentPollCount = 0;
            else if (serializableSpans.Count > 0) subsequentPollCount++;

            if (ShouldSendQueuedSpansOverWire())
            {
                SendSpansOverHttp();
            }
        }

        /// <summary>
        /// 是否发送信息到zipkin服务器
        /// </summary>
        /// <returns></returns>
        private bool ShouldSendQueuedSpansOverWire()
        {
            //return 有追踪信息 &&(信息数量超过成批提交的数量 || 任务已取消 || 超过MAX_NUMBER_OF_POLLS次没有新信息)
            return
#if NET20 || NET35
                serializableSpans.Count > 0 &&
                   (serializableSpans.Count >= maxBatchSize
#else
                serializableSpans.Any() &&
                   (serializableSpans.Count() >= maxBatchSize
#endif


                   || spanProcessorTaskFactory.IsTaskCancelled()
                   || subsequentPollCount > MAX_NUMBER_OF_POLLS);
        }

        private bool ProcessQueuedSpans()
        {
            Span span;
            var anyNewSpansQueued = false;
            while (spanQueue.TryTake(out span))
            {
                serializableSpans.Enqueue(new JsonSpan(span));
                anyNewSpansQueued = true;
            }
            return anyNewSpansQueued;
        }

        private void SendSpansOverHttp()
        {
            var spansJsonRepresentation = GetSpansJSONRepresentation();
            SendSpansToZipkin(spansJsonRepresentation);
            subsequentPollCount = 0;
        }

        public virtual
#if COREFX
            async
#endif
             void SendSpansToZipkin(string spans)
        {
            if (spans == null) throw new ArgumentNullException("spans");
#if COREFX
            await httpClient.PostAsync(uri, new System.Net.Http.StringContent(spans));
#else
            //Harry.Common.HttpHelper.Post(this.uriString, spans, Encoding.UTF8);
            using (var client = new WebClient())
            {
                try
                {
                    //client.BaseAddress = uri.ToString();
                    client.Encoding = Encoding.UTF8;
                    client.UploadString(uri, "POST", spans);
                }
                catch (WebException ex)
                {
                    //Very friendly HttpWebRequest Error message with good information.
                    LogHttpErrorMessage(ex);
                    throw;
                }
            }
#endif


        }

        private string GetSpansJSONRepresentation()
        {
            JsonSpan span;
            var spanList = new List<JsonSpan>();
            //using Dequeue into a list so that the span is removed from the queue as we add it to list
            while (serializableSpans.TryDequeue(out span))
            {
                spanList.Add(span);
            }
            var spansJsonRepresentation = JsonConvert.SerializeObject(spanList);
            return spansJsonRepresentation;
        }

        private void LogHttpErrorMessage(WebException ex)
        {
            var response = ex.Response as HttpWebResponse;
            if ((response == null)) return;
            var responseStream = response.GetResponseStream();
            var responseString = responseStream != null ? new System.IO.StreamReader(responseStream).ReadToEnd() : string.Empty;
            logger.Log(Logging.LogLevel.Error, 0, ex, string.Format(
               "Failed to send spans to Zipkin server (HTTP status code returned: {0}). Exception message: {1}, response from server: {2}",
               response.StatusCode, ex.Message, responseString));
        }
    }
}
