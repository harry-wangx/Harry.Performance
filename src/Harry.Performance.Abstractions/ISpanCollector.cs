//-----------------------------------------------------------------------
//收集器接口
//-----------------------------------------------------------------------
#if ASYNC
using System.Threading.Tasks;
#endif


namespace Harry.Performance
{
    public interface ISpanCollector
    {
        void Collect(params ISpan[] spans);

        void OnStart();

        void OnComplete(long elapsedMilliseconds);

    }
}
