using UFramework.Core;
public class ProviderBootStart : IBootstrap {
    public void Bootstrap () {
        IServiceProvider[] providerArray = new IServiceProvider[] {
            new ProviderLogService ()
        };

        foreach (IServiceProvider provider in providerArray) {
            if (provider == null) {
                continue;
            }

            if (App.IsRegistered (provider)) {
                continue;
            }

            App.Register (provider);
        }
    }
}