    using System;
    namespace UFramework.Promise {
        internal class PredicateWait {

            public IPendingPromise pendingPromise;
            public Func<float, bool> predicate;
            public float createTime;

            public float alreadyWaitTime;
        }
    }