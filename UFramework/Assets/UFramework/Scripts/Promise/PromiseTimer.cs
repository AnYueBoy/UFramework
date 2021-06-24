namespace UFramework.Promise {
    using System.Collections.Generic;
    using System;

    public class PromiseTimer : IPromiseTimer {
        private float curTime;
        private List<PredicateWait> waitingList = new List<PredicateWait> ();

        public void localUpdate (float deltaTime) {
            this.curTime += deltaTime;
            int index = 0;
            while (index < this.waitingList.Count) {
                PredicateWait wait = this.waitingList[index];
                // 当前wait从创建开始到现在的时间(存在时间)
                float alreadyWaitTime = this.curTime - wait.createTime;
                // 已用时间
                wait.alreadyWaitTime = alreadyWaitTime;

                bool flag;
                try {
                    // 达成条件
                    flag = wait.predicate (wait.alreadyWaitTime);
                } catch (Exception exception) {
                    wait.pendingPromise.reject (exception);
                    this.waitingList.Remove (wait);
                    index = 0;
                    continue;
                }

                if (flag) {
                    wait.pendingPromise.resolve ();
                    this.waitingList.Remove (wait);
                    index = 0;
                }

                index++;
            }
        }

        public IPromise waitFor (float seconds) {
            return this.waitUtil ((float alreadyTime) => {
                return alreadyTime >= seconds;
            });
        }

        public IPromise waitUtil (Func<float, bool> predicate) {
            Promise promise = new Promise ();
            PredicateWait item = new PredicateWait {
                createTime = this.curTime,
                pendingPromise = promise,
                predicate = predicate,
                alreadyWaitTime = 0
            };

            this.waitingList.Add (item);
            return promise;
        }
    }
}