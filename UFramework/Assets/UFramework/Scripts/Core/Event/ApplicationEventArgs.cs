using UFramework.EventDispatcher;

namespace UFramework.Core {
    /// <summary>
    /// Represents an application event.
    /// </summary>
    public class ApplicationEventArgs : EventParam {
        /// <summary>
        /// Initializes a new instance of the ApplicationEventArgs class.
        /// </summary>
        public ApplicationEventArgs (IApplication application) {
            Application = application;
        }

        /// <summary>
        /// Gets the application instance.
        /// </summary>
        public IApplication Application { get; private set; }
    }
}