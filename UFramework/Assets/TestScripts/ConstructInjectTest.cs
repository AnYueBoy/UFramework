using UnityEngine;

namespace UFramework
{
    public class ConstructInjectTest
    {
        private readonly ICoroutineManager coroutineManager;

        public ConstructInjectTest(ICoroutineManager coroutineManager)
        {
            this.coroutineManager = coroutineManager;
        }

        public void Init()
        {
            Debug.LogError($"{coroutineManager == null}");
        }
    }
}