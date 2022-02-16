namespace UFramework.Core {
    /// <summary>
    /// ServiceProvider is default service provider class
    /// for all concrete ServiceProvider classes.
    /// </summary>
    public abstract class ServiceProvider : IServiceProvider {
        /// <summary>
        /// Gets application instance.
        /// </summary>
        protected IApplication App { get; private set; }

        /// <inheritdoc />
        public virtual void Init () { }

        /// <inheritdoc />
        public virtual void Register () { }

        internal void SetApplication (IApplication application) {
            App = application;
        }
    }
}