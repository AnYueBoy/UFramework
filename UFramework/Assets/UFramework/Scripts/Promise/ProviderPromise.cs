namespace UFramework
{
    public class ProviderPromise : IServiceProvider
    {
        public void Init()
        {
        }

        public void Register()
        {
            App.Singleton<IPromiseTimer, PromiseTimer>();
        }
    }
}