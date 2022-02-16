namespace UFramework.Core {
    /// <summary>
    /// Indicates that all the IServiceProvider.Init has been called.
    /// </summary>
    public class AfterInitEventArgs : ApplicationEventArgs {
        /// <summary>
        /// Initializes a new instance of the IServiceProvider.Init class.
        /// </summary>
        public AfterInitEventArgs (IApplication application) : base (application) { }
    }
}