using UFramework.Container;
using UFramework.EventDispatcher;

namespace UFramework.Core {
    public interface IApplication : IContainer {

        /// <summary>
        /// Gets a value indicating whether true if we're on the main thread.
        /// </summary>
        bool IsMainThread { get; }

        /// <summary>
        /// Gets or sets the debug level.
        /// </summary>
        DebugLevel DebugLevel { get; set; }

        /// <summary>
        /// Gets the event dispatcher.
        /// </summary>
        /// <returns>Returns event dispathcher instance, null if the dispatcher not found.</returns>
        IEventDispatcher GetDispatcher ();

        /// <summary>
        /// Register a service provider with the application.
        /// </summary>
        void Register (IServiceProvider provider, bool force = false);

        /// <summary>
        /// Checks whether the given service provider is registered.
        /// </summary>
        bool IsRegistered (IServiceProvider provider);

        /// <summary>
        /// Gets the unique runtime id.
        /// </summary>
        long GetRuntimeId ();

        /// <summary>
        /// Terminates the IApplication
        /// </summary>
        void Terminate ();
    }
}