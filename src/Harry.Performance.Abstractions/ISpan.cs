using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Harry.Performance
{
    /// <summary>
    /// A set of <see cref="Annotation"/> and <see cref="BinaryAnnotation"/> elements that correspond to a particular RPC. 
    /// Spans contain identifying information such as traceId, spandId, parentId, and RPC name.
    /// </summary>
    public interface ISpan
    {

        /// <summary>
        /// Records an annotation within current span. 
        /// Also sets it's endpoint if it was not set previously.
        /// </summary>
        void Record(Annotation annotation);

        /// <summary>
        /// Records a binary annotation within current span. 
        /// Also sets it's endpoint if it was not set previously.
        /// </summary>
        void Record(BinaryAnnotation binaryAnnotation);


    }
}
