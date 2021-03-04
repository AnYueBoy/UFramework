namespace UFramework.Promise {
    using System;

    internal class PredicateWait {

        public IPendingPromise pendingPromise;
        public Func<TimeData, bool> predicate;
        public TimeData timeData;
        public float timeStarted;
    }
}