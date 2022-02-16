namespace UFramework.Core {
    /// <summary>
    /// Indicates a service provider that will inited.
    /// </summary>
    public class InitProviderEventArgs : ApplicationEventArgs {
        private readonly IServiceProvider provider;

        /// <summary>
        /// Initializes a new instance of the InitProviderEventArgs class.
        /// </summary>
        public InitProviderEventArgs (IServiceProvider provider, IApplication application) : base (application) {
            this.provider = provider;
        }

        /// <summary>
        /// Gets the a service provider class that will inited.
        /// </summary>
        public IServiceProvider GetServiceProvider () {
            return provider;
        }
    }
}