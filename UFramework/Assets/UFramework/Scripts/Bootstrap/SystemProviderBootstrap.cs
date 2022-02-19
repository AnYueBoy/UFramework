using UFramework.Core;
using UFramework.GameCommon;
using UFramework.Promise;
using UFramework.Tween;
namespace UFramework.Bootstarp {

    public class SystemProviderBootstrap : IBootstrap {
        public void Bootstrap () {

            IServiceProvider[] providerArray = new IServiceProvider[] {
                new ProviderPromise (),
                new ProviderTweener (),
                new ProviderGameCommon (),
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

}