using System;
using UFramework.Util;
using UFrameworkContainer = UFramework.Container.Container;

namespace UFramework.Container {
    public sealed class BindData : Bindable, IBindData {

        public BindData (
            Container container,
            string service,
            Func<IContainer, object[], object> concrete,
            bool isStatic) : base (container, service) {
            Concrete = concrete;
            IsStatic = isStatic;
        }

        public Func<IContainer, object[], object> Concrete { get; }

        public bool IsStatic { get; }

        public IBindData Alias (string alias) {
            AssertDestroyed ();
            Guard.ParameterNotNull (alias, nameof (alias));
            Container.Alias (alias, Service);
            return this;
        }

        public IBindData Alias<TAlias> () {
            return this.Alias (this.Container.Type2Service (typeof (TAlias)));
        }

        public IBindData Tag (string tag) {
            AssertDestroyed ();
            Guard.ParameterNotNull (tag, nameof (tag));
            Container.Tag (tag, Service);
            return this;
        }

        protected override void ReleaseBind () {
            ((UFrameworkContainer) Container).Unbind (this);
        }
    }
}