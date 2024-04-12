namespace UFramework
{
    public class ProviderGuideManager : IServiceProvider
    {
        public void Register()
        {
            App.Singleton<IGuideManager>(() => new GuideManager());
        }

        public void Init()
        {
        }
    }
}