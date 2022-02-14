using System;
using UFramework.Exception;
using UFramework.Util;
using UFrameworkContainer = UFramework.Container.Container;

namespace UFramework.Container {
    public sealed class BindData : IBindData {

        private readonly Container container;
        private bool isDestroy;
        public string Service { get; }
        public Func<IContainer, object[], object> Concrete { get; }
        public bool IsStatic { get; }
        public IContainer Container => this.container;

        public BindData (
            Container container,
            string service,
            Func<IContainer, object[], object> concrete,
            bool isStatic) {

            this.container = container;
            Service = service;
            this.isDestroy = false;

            Concrete = concrete;
            IsStatic = isStatic;
        }

        public IBindData Alias (string alias) {
            AssertDestroyed ();
            Guard.ParameterNotNull (alias, nameof (alias));
            Container.Alias (alias, Service);
            return this;
        }

        public IBindData Alias<TAlias> () {
            return this.Alias (this.Container.Type2Service (typeof (TAlias)));
        }

        public void Unbind () {
            isDestroy = true;
            ((UFrameworkContainer) Container).Unbind (this);
        }

        private void AssertDestroyed () {
            if (isDestroy) {
                throw new LogicException ("The current instance is destroyed.");
            }
        }
    }
}