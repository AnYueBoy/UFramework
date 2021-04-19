namespace UFramework.Promise {
    using System;

    public interface IPromiseTimer {
        void localUpdate (float deltaTime);
        IPromise waitFor (float seconds);
        IPromise waitUtil (Func<TimeData, bool> predicate);
        IPromise waitWhile (Func<TimeData, bool> predicate);
    }
}