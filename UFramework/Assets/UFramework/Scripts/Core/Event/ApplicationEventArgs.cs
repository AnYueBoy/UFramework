using System;

namespace UFramework.Core
{
    /// <summary>
    /// Represents an application event.
    /// </summary>
    public class ApplicationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationEventArgs"/> class.
        /// </summary>
        /// <param name="application">The application instance.</param>
        public ApplicationEventArgs(IApplication application)
        {
            Application = application;
        }

        /// <summary>
        /// Gets the application instance.
        /// </summary>
        public IApplication Application { get; private set; }
    }
}
