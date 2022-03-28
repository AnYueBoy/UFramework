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
            isDestroy = false;

            Concrete = concrete;
            IsStatic = isStatic;
        }

        public void Unbind () {
            isDestroy = true;
            ((UFrameworkContainer) Container).Unbind (this);
        }
    }
}