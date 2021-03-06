namespace UFramework.Promise {
    using System.Collections.Generic;
    using System;

    public class PromiseTimer : IPromiseTimer {
        private float curTime;
        private List<PredicateWait> waitingList = new List<PredicateWait> ();
        public void update (float deltaTime) {
            this.curTime += deltaTime;
            int index = 0;
            while (index < this.waitingList.Count) {
                bool flag;
                PredicateWait wait = this.waitingList[index];

                // 当前wait从创建开始到现在的时间(存在时间)
                float alreadyWaitTime = this.curTime - wait.timeStarted;

                // 存在时间 - wait的活跃时间 = wait的不活跃时间
                wait.timeData.deltaTime = alreadyWaitTime - wait.timeData.elapsedTime;
                // FIXME: 不明确
                wait.timeData.elapsedTime = alreadyWaitTime;

                try {
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
            throw new NotImplementedException ();
        }

        public IPromise waitUtil (Func<TimeData, bool> predicate) {
            throw new NotImplementedException ();
        }

        public IPromise waitWhile (Func<TimeData, bool> predicate) {
            throw new NotImplementedException ();
        }
    }
}