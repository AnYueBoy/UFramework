using System.Collections;
using System.Collections.Generic;

namespace UFramework
{
    public class CoroutineManager : ICoroutineManager
    {
        private List<ICoroutine> _coroutines;
        private Queue<ICoroutine> _recycle;

        public void Init()
        {
            _coroutines = new List<ICoroutine>();
            _recycle = new Queue<ICoroutine>();
        }

        public void LocalUpdate(float dt)
        {
            var count = _recycle.Count;
            for (int i = 0; i < count; i++)
            {
                var _coroutine = _recycle.Dequeue();
                _coroutines.Remove(_coroutine);
            }

            count = _coroutines.Count;
            for (int i = 0; i < count; i++)
            {
                var _coroutine = _coroutines[i];
                if (_coroutine.State != CoroutineState.Working)
                {
                    continue;
                }

                if (!(_coroutine as Coroutine).IsDone)
                {
                    continue;
                }

                _coroutine.Complete();
            }
        }

        public void SetRecycle(ICoroutine routine)
        {
            _recycle.Enqueue(routine);
        }

        public void Terminate()
        {
            _coroutines.Clear();
            _recycle.Clear();
        }

        public ICoroutine CreateCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = new Coroutine();
            coroutine._routine = routine;
            PauseCoroutine(coroutine);
            return coroutine;
        }

        public ICoroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = CreateCoroutine(routine);
            _coroutines.Add(coroutine);
            ResumeCoroutine(coroutine);
            return coroutine;
        }

        public void PauseCoroutine(ICoroutine routine)
        {
            routine.Pause();
        }

        public void ResumeCoroutine(ICoroutine coroutine)
        {
            coroutine.Resume();
        }

        public void StopCoroutine(ICoroutine coroutine)
        {
            coroutine.Complete();
        }
    }
}