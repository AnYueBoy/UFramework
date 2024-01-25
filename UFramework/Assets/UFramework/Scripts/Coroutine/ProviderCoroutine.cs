namespace UFramework
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