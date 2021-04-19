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
                bool flag;
                PredicateWait wait = this.waitingList[index];

                // 当前wait从创建开始到现在的时间(存在时间)
                float alreadyWaitTime = this.curTime - wait.timeStarted;
                // 帧时间
                wait.timeData.deltaTime = alreadyWaitTime - wait.timeData.elapsedTime;
                // 已用时间
                wait.timeData.elapsedTime = alreadyWaitTime;

                try {
                    // 达成条件
                    flag = wait.predicate (wait.timeData);
                } catch (Exception exception) {
                    wait.pendingPromise.reject (exception);
                    this.waitingList.RemoveAt (index);
                    continue;
                }

                if (flag) {
                    wait.pendingPromise.resolve ();
                    this.waitingList.RemoveAt (index);
                } else {
                    index++;
                }
            }
        }

        public IPromise waitFor (float seconds) {
            return this.waitUtil ((TimeData timeInfo) => {
                return timeInfo.elapsedTime >= seconds;
            });
        }

        public IPromise waitWhile (Func<TimeData, bool> predicate) {
            return this.waitUtil ((TimeData timeInfo) => {
                return !predicate (timeInfo);
            });
        }

        public IPromise waitUtil (Func<TimeData, bool> predicate) {
            Promise promise = new Promise ();
            PredicateWait item = new PredicateWait {
                timeStarted = this.curTime,
                pendingPromise = promise,
                timeData = new TimeData (),
                predicate = predicate
            };

            this.waitingList.Add (item);
            return promise;
        }
    }
}