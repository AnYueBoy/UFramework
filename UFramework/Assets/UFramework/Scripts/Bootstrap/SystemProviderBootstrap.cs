using UFramework.Core;
using UFramework.Promise;
using UFramework.Tween;
namespace UFramework.Bootstarp {

    public class SystemProviderBootstrap : IBootstrap {
        public void Bootstrap () {

            IServiceProvider[] providerArray = new IServiceProvider[] {
                new ProviderPromise (),
                new ProviderTweener (),
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

}