

namespace UFramework.Core
{
    /// <summary>
    /// Indicates a service provider that will inited.
    /// </summary>
    public class InitProviderEventArgs : ApplicationEventArgs
    {
        private readonly IServiceProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitProviderEventArgs"/> class.
        /// </summary>
        /// <param name="provider">The service provider class that will inited.</param>
        /// <param name="application">The application instance.</param>
        public InitProviderEventArgs(IServiceProvider provider, IApplication application)
            : base(application)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Gets the a service provider class that will inited.
        /// </summary>
        /// <returns>Return the service provider class.</returns>
        public IServiceProvider GetServiceProvider()
        {
            return provider;
        }
    }
}
