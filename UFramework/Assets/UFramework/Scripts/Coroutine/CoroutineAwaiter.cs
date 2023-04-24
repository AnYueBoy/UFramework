using System;
using System.Collections.Generic;

namespace UFramework.Coroutine
{
    public struct CoroutineAwaiter : IAwaiter, ICriticalAwaiter
    {
        private Coroutine cor;
        private Queue<Action> calls;

        public CoroutineAwaiter(ICoroutine cor)
        {
            this.cor = cor as Coroutine;
            calls = new Queue<Action>();
            this.cor.onCompleted += TaskCompleted;
        }

        private void TaskCompleted()
        {
            while (calls.Count != 0)
            {
                calls.Dequeue()?.Invoke();
            }
        }

        public bool IsCompleted => cor.IsDone;

        public void GetResult()
        {
            if (!IsCompleted)
            {
                throw new System.Exception("The task is not finished yet.");
            }
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException("continuation");
            }

            calls.Enqueue(continuation);
        }
    }
}