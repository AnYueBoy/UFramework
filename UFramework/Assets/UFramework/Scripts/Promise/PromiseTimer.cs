using System.Collections.Generic;
using System;
using SException = System.Exception;

namespace UFramework
{
    public class PromiseTimer : IPromiseTimer
    {
        private float curTime;
        private List<PredicateWait> waitingList = new List<PredicateWait>();

        public void LocalUpdate(float deltaTime)
        {
            curTime += deltaTime;
            int index = 0;
            while (index < waitingList.Count)
            {
                PredicateWait wait = waitingList[index];
                // 当前wait从创建开始到现在的时间(存在时间)
                float alreadyWaitTime = curTime - wait.createTime;
                // 已用时间
                wait.alreadyWaitTime = alreadyWaitTime;

                bool flag;
                try
                {
                    // 达成条件
                    flag = wait.predicate(wait.alreadyWaitTime);
                }
                catch (SException exception)
                {
                    wait.pendingPromise.Reject(exception);
                    waitingList.Remove(wait);
                    index = 0;
                    continue;
                }

                if (flag)
                {
                    wait.pendingPromise.Resolve();
                    waitingList.Remove(wait);
                    index = 0;
                }

                index++;
            }
        }

        public IPromise WaitFor(float seconds)
        {
            return WaitUtil((float alreadyTime) => { return alreadyTime >= seconds; });
        }

        public IPromise WaitUtil(Func<float, bool> predicate)
        {
            Promise promise = new Promise();
            PredicateWait item = new PredicateWait
            {
                createTime = curTime,
                pendingPromise = promise,
                predicate = predicate,
                alreadyWaitTime = 0
            };

            waitingList.Add(item);
            return promise;
        }
    }
}