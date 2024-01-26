using UnityEngine;

namespace UFramework
{
    public class SystemProviderBootstrap : IBootstrap
    {
        private IServiceProvider[] unityProviderArray;
        private IServiceProvider[] customProviderArray;

        public SystemProviderBootstrap(Component component)
        {
            unityProviderArray = component.GetComponentsInChildren<IServiceProvider>();
        }

        public void AddCustomProviders(params IServiceProvider[] customProviders)
        {
            customProviderArray = customProviders;
        }

        public void Bootstrap()
        {
            IServiceProvider[] providerArray =
                Arr.Merge<IServiceProvider>(unityProviderArray, customProviderArray);

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