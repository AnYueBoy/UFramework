namespace UFramework.Core {
    /// <summary>
    /// Indicates that the bootstrap has been booted.
    /// </summary>
    public class AfterBootEventArgs : ApplicationEventArgs {
        /// <summary>
        /// Initializes a new instance of the AfterBootEventArgs class.
        /// </summary>
        /// <param name="application">The application instance.</param>
        public AfterBootEventArgs (IApplication application) : base (application) { }
    }
}