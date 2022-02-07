namespace UFramework.Core
{
    /// <summary>
    /// Indicates that the framework ready.
    /// </summary>
    public class StartCompletedEventArgs : ApplicationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="application">The application instance.</param>
        public StartCompletedEventArgs(IApplication application)
            : base(application)
        {
        }
    }
}
