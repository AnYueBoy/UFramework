using System;

namespace UFramework
{
    public class ResolveHandler
    {
        public Action callback;
        public IRejectable rejectable;
    }

    public class ResolveHandler<PromiseT>
    {
        public Action<PromiseT> callback;
        public IRejectable rejectable;
    }
}