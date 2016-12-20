//-----------------------------------------------------------------------
//收集器接口
//-----------------------------------------------------------------------

namespace Harry.Performance
{
    public interface ISpanCollector: ICollector
    {
        void Collect(Span[] spans);

    }
}
