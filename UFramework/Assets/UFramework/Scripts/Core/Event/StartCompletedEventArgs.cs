namespace UFramework.Core {
    /// <summary>
    /// Indicates that the framework ready.
    /// </summary>
    public class StartCompletedEventArgs : ApplicationEventArgs {
        /// <summary>
        /// Initializes a new instance of the StartCompletedEventArgs class.
        /// </summary>
        public StartCompletedEventArgs (IApplication application) : base (application) { }
    }
}