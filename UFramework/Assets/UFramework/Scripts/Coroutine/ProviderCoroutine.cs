namespace UFramework
{
    public class ProviderCoroutine : IServiceProvider
    {
        public void Register()
        {
            App.Singleton<ICoroutineManager>(() => new CoroutineManager());
        }

        public void Init()
        {
            App.Make<ICoroutineManager>().Init();
        }
    }
}