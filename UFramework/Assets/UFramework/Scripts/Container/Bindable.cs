using UFramework.Exception;

namespace UFramework.Container {

    /// <summary>
    /// The bindable data indicates relational data related to the specified service.
    /// </summary>
    public abstract class Bindable : IBindable {

        private readonly Container container;
        private bool isDestroy;

        protected Bindable (Container container, string service) {
            this.container = container;
            Service = service;
            isDestroy = false;
        }

        public string Service { get; }

        public IContainer Container => container;

        public void Unbind () {
            isDestroy = true;
            ReleaseBind ();
        }

        protected abstract void ReleaseBind ();

        protected void AssertDestroyed () {
            if (isDestroy) {
                throw new LogicException ("The current instance is destroyed.");
            }
        }
    }
}