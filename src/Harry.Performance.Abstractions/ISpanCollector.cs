//-----------------------------------------------------------------------
//收集器接口
//-----------------------------------------------------------------------
#if !NET20 && !NET35
using System.Threading.Tasks;
#endif


namespace Harry.Performance
{
    public interface ISpanCollector
    {
        void Collect(params Span[] spans);

#if !NET20 && !NET35
        Task CollectAsync(params Span[] spans);
#endif


    }
}
