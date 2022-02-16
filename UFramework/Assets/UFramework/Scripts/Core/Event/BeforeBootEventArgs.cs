namespace UFramework.Core {
    /// <summary>
    /// It indicates that the bootstrap will be bootstrapped.
    /// </summary>
    public class BeforeBootEventArgs : ApplicationEventArgs {
        private IBootstrap[] bootstraps;

        /// <summary>
        /// Initializes a new instance of the BeforeBootEventArgs class.
        /// </summary>
        public BeforeBootEventArgs (IBootstrap[] bootstraps, IApplication application) : base (application) {
            this.bootstraps = bootstraps;
        }

        /// <summary>
        /// Gets an array of bootstrap will be bootstrapped.
        /// </summary>
        public IBootstrap[] GetBootstraps () {
            return bootstraps;
        }

        /// <summary>
        /// Sets the bootstrap will replace the old boot list.
        /// </summary>
        public void SetBootstraps (IBootstrap[] bootstraps) {
            this.bootstraps = bootstraps;
        }
    }
}