namespace UFramework.Core {
    /// <summary>
    /// Indicates that the framework will terminate.
    /// </summary>
    public class AfterTerminateEventArgs : ApplicationEventArgs {
        /// <summary>
        /// Initializes a new instance of the AfterTerminateEventArgs class.
        /// </summary>
        public AfterTerminateEventArgs (IApplication application) : base (application) { }
    }
}