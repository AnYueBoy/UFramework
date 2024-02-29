using UFramework;

public class ProviderRedDotSystem : IServiceProvider
{
    public void Register()
    {
        App.Singleton<IRedDotSystem>(() => new RedDotSystem());
    }

    public void Init()
    {
        App.Make<IRedDotSystem>().RegisterTrigger();
    }
}