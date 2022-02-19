    using System;
    namespace UFramework.Promise {

        public interface IPromiseTimer {
            void LocalUpdate (float deltaTime);
            IPromise WaitFor (float seconds);
            IPromise WaitUtil (Func<float, bool> predicate);
        }
    }