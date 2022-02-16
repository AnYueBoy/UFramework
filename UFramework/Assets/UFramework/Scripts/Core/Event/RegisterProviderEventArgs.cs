using UFramework.EventDispatcher;

namespace UFramework.Core {
    /// <summary>
    /// Indicates a service provider that will register.
    /// </summary>
    public class RegisterProviderEventArgs : ApplicationEventArgs, IStoppableEvent {
        private readonly IServiceProvider provider;

        /// <summary>
        /// Initializes a new instance of the RegisterProviderEventArgs class.
        /// </summary>
        public RegisterProviderEventArgs (IServiceProvider provider, IApplication application) : base (application) {
            IsSkip = false;
            this.provider = provider;
        }

        /// <summary>
        /// Gets a value indicating whether the service provider is skip register.
        /// </summary>
        public bool IsSkip { get; private set; }

        public bool IsPropagationStopped => IsSkip;

        /// <summary>
        /// Gets the a service provider class that will register.
        /// </summary>
        public IServiceProvider GetServiceProvider () {
            return provider;
        }

        /// <summary>
        /// Skip the register service provider.
        /// </summary>
        public void Skip () {
            IsSkip = true;
        }
    }
}