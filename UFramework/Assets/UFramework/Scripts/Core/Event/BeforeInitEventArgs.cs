

namespace UFramework.Core
{
    /// <summary>
    /// It indicates that the <see cref="IServiceProvider.Init"/> method will be called.
    /// </summary>
    public class BeforeInitEventArgs : ApplicationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeInitEventArgs"/> class.
        /// </summary>
        /// <param name="bootstraps">An array of the bootstrap list.</param>
        /// <param name="application">The application instance.</param>
        public BeforeInitEventArgs(IApplication application)
            : base(application)
        {
        }
    }
}
