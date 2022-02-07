
namespace UFramework.Core
{
    /// <summary>
    /// Indicates that all the <see cref="IServiceProvider.Init"/> has been called.
    /// </summary>
    public class AfterInitEventArgs : ApplicationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterInitEventArgs"/> class.
        /// </summary>
        /// <param name="application">The application instance.</param>
        public AfterInitEventArgs(IApplication application)
            : base(application)
        {
        }
    }
}
