    using System;
    namespace UFramework.Promise {

        public interface IPromiseTimer {
            void localUpdate (float deltaTime);
            IPromise waitFor (float seconds);
            IPromise waitUtil (Func<float, bool> predicate);
        }
    }