namespace UFramework.Core {
    public abstract class ServiceProvider : IServiceProvider {
        protected IApplication App { get; private set; }

        public virtual void Init () { }

        public virtual void Register () { }

        internal void SetApplication (IApplication application) {
            App = application;
        }
    }
}