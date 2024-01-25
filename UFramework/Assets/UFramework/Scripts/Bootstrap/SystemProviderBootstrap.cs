using UnityEngine;

namespace UFramework
{
    public class SystemProviderBootstrap : IBootstrap
    {
        private IServiceProvider[] unityProviderArray;

        public SystemProviderBootstrap(Component component)
        {
            unityProviderArray = component.GetComponentsInChildren<IServiceProvider>();
        }

        public void Bootstrap()
        {
            IServiceProvider[] systemProviderArray = new IServiceProvider[]
            {
                new ProviderPromise(),
                new ProviderTweener(),
                new ProviderGameCommon(),
                new ProviderLogManager(),
                new ProviderCoroutine(),
            };

            IServiceProvider[] providerArray = Arr.Merge<IServiceProvider>(unityProviderArray, systemProviderArray);

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
}