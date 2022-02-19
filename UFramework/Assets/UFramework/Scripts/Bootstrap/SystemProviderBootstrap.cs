using UFramework.Core;
using UFramework.GameCommon;
using UFramework.Promise;
using UFramework.Tween;
using UFramework.LogSystem;
namespace UFramework.Bootstarp {

    public class SystemProviderBootstrap : IBootstrap {
        public void Bootstrap () {

            IServiceProvider[] providerArray = new IServiceProvider[] {
                new ProviderPromise (),
                new ProviderTweener (),
                new ProviderGameCommon (),
                new ProviderLogManager ()
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