namespace UFramework
{
    public class ProviderContructInject : IServiceProvider
    {
        public void Register()
        {
            App.Singleton<ConstructInjectTest>();
        }

        public void Init()
        {
            App.Make<ConstructInjectTest>().Init();
        }
    }
}