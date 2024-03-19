using System.Collections;
using System.Collections.Generic;

namespace UFramework
{
    public class CoroutineManager : ICoroutineManager
    {
        private List<ICoroutine> _coroutines;
        private Queue<ICoroutine> _recycle;

        private List<ICoroutine> _coroutinePool;

        public void Init()
        {
            _coroutines = new List<ICoroutine>();
            _recycle = new Queue<ICoroutine>();
            _coroutinePool = new List<ICoroutine>();
        }

        public void LocalUpdate(float dt)
        {
            var count = _recycle.Count;
            for (int i = 0; i < count; i++)
            {
                var _coroutine = _recycle.Dequeue();
                _coroutines.Remove(_coroutine);
                _coroutinePool.Add(_coroutine);
            }

            count = _coroutines.Count;
            for (int i = 0; i < count; i++)
            {
                var _coroutine = _coroutines[i];
                if (_coroutine.State == CoroutineState.Working)
                {
                    if ((_coroutine as Coroutine).IsDone)
                    {
                        _coroutine.Complete();
                    }
                }
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
            _coroutinePool.Clear();
        }

        public ICoroutine CreateCoroutine(IEnumerator routine)
        {
            Coroutine coroutine;
            if (_recycle.Count > 0)
            {
                coroutine = _coroutinePool[0] as Coroutine;
                _coroutinePool.RemoveAt(0);
            }
            else
            {
                coroutine = new Coroutine();
            }

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