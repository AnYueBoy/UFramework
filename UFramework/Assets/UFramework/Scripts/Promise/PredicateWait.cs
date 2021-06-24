namespace UFramework.Promise {
    using System;

    internal class PredicateWait {

        public IPendingPromise pendingPromise;
        public Func<float, bool> predicate;
        public float createTime;

        public float alreadyWaitTime;
    }
}