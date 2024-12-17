using System;
using System.Collections.Generic;

namespace UFramework
{
    public abstract class Bindable : IBindable
    {
        private readonly Container container;
        private Dictionary<string, Func<object>> contextualClosure;
        private bool isDestroy;

        protected Bindable(Container container, string service)
        {
            this.container = container;
            Service = service;
            isDestroy = false;
        }

        public string Service { get; }
        public IContainer Container => container;

        public void Unbind()
        {
            isDestroy = true;
            ReleaseBind();
        }

        protected abstract void ReleaseBind();

        internal Func<object> GetContextualClosure(string needs)
        {
            if (contextualClosure == null)
            {
                return null;
            }

            return contextualClosure.TryGetValue(needs, out Func<object> closure) ? closure : null;
        }

        protected void AssertDestroyed()
        {
            if (isDestroy)
            {
                throw new LogicException("当前实例已销毁");
            }
        }
    }
}