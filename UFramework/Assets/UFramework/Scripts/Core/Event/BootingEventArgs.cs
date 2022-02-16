using UFramework.EventDispatcher;

namespace UFramework.Core {
    /// <summary>
    /// Indicates a boot class that is booting.
    /// </summary>
    public class BootingEventArgs : ApplicationEventArgs, IStoppableEvent {
        private readonly IBootstrap bootstrap;

        /// <summary>
        /// Initializes a new instance of the BootingEventArgs class.
        /// </summary>
        public BootingEventArgs (IBootstrap bootstrap, IApplication application) : base (application) {
            IsSkip = false;
            this.bootstrap = bootstrap;
        }

        /// <summary>
        /// Gets a value indicating whether the boot class is skip booting.
        /// </summary>
        public bool IsSkip { get; private set; }

        /// <inheritdoc />
        public bool IsPropagationStopped => IsSkip;

        /// <summary>
        /// Gets the a boot class that is booting.
        /// </summary>
        public IBootstrap GetBootstrap () {
            return bootstrap;
        }

        /// <summary>
        /// Disable the boot class.
        /// </summary>
        public void Skip () {
            IsSkip = true;
        }
    }
}