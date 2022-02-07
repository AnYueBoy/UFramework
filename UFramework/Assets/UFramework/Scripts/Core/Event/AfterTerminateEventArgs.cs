namespace UFramework.Core
{
    /// <summary>
    /// Indicates that the framework will terminate.
    /// </summary>
    public class AfterTerminateEventArgs : ApplicationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterTerminateEventArgs"/> class.
        /// </summary>
        /// <param name="application">The terminate application instance.</param>
        public AfterTerminateEventArgs(IApplication application)
            : base(application)
        {
        }
    }
}
