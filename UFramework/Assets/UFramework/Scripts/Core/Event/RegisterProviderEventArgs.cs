
using UFramework.EventDispatcher;

namespace UFramework.Core
{
    /// <summary>
    /// Indicates a service provider that will register.
    /// </summary>
    public class RegisterProviderEventArgs : ApplicationEventArgs, IStoppableEvent
    {
        private readonly IServiceProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterProviderEventArgs"/> class.
        /// </summary>
        /// <param name="provider">The service provider class that will register.</param>
        /// <param name="application">The application instance.</param>
        public RegisterProviderEventArgs(IServiceProvider provider, IApplication application)
            : base(application)
        {
            IsSkip = false;
            this.provider = provider;
        }

        /// <summary>
        /// Gets a value indicating whether the service provider is skip register.
        /// </summary>
        public bool IsSkip { get; private set; }

        /// <inheritdoc />
        public bool IsPropagationStopped => IsSkip;

        /// <summary>
        /// Gets the a service provider class that will register.
        /// </summary>
        /// <returns>Return the service provider class.</returns>
        public IServiceProvider GetServiceProvider()
        {
            return provider;
        }

        /// <summary>
        /// Skip the register service provider.
        /// </summary>
        public void Skip()
        {
            IsSkip = true;
        }
    }
}
