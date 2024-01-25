using UFramework;

public class ProviderBootStart : IBootstrap
{
    public void Bootstrap()
    {
        IServiceProvider[] providerArray = new IServiceProvider[]
        {
            // new ProviderLogService (),
            new ProviderTransform()
        };

        foreach (IServiceProvider provider in providerArray)
        {
            if (provider == null)
            {
                continue;
            }

            if (App.IsRegistered(provider))
            {
                continue;
            }

            App.Register(provider);
        }
    }
}