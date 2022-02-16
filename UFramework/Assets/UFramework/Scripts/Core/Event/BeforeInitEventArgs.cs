namespace UFramework.Core {
    /// <summary>
    /// It indicates that the IServiceProvider.Init method will be called.
    /// </summary>
    public class BeforeInitEventArgs : ApplicationEventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeInitEventArgs"/> class.
        /// </summary>
        public BeforeInitEventArgs (IApplication application) : base (application) { }
    }
}