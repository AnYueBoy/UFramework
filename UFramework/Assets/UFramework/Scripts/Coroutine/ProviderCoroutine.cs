using UFramework.Core;

namespace UFramework.Coroutine
{
    public class ProviderCoroutine : IServiceProvider
    {
        public void Register()
        {
            App.Singleton<ICoroutineManager, CoroutineManager>();
        }

        public void Init()
        {
            App.Make<ICoroutineManager>().Init();
        }
    }
}