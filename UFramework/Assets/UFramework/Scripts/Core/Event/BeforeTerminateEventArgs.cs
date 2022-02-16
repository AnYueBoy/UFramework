namespace UFramework.Core {
    /// <summary>
    /// Indicates that the framework will terminate.
    /// </summary>
    public class BeforeTerminateEventArgs : ApplicationEventArgs {
        /// <summary>
        /// Initializes a new instance of the BeforeTerminateEventArgs class.
        /// </summary>
        /// <param name="application">The terminate application instance.</param>
        public BeforeTerminateEventArgs (IApplication application) : base (application) { }
    }
}