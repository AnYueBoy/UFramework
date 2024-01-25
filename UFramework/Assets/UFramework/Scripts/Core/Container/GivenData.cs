using System;

namespace UFramework
{
    internal sealed class GivenData<TReturn> : IGivenData<TReturn> where TReturn : class, IBindable<TReturn>
    {
        private readonly Bindable<TReturn> bindable;
        private readonly Container container;
        private string needs;

        public GivenData(Container container, Bindable<TReturn> bindable)
        {
            this.container = container;
            this.bindable = bindable;
        }

        /// <summary>
        /// 绑定服务名与上下文关系
        /// </summary>
        public TReturn Given(string service)
        {
            Guard.ParameterNotNull(service, nameof(service));
            bindable.AddContextual(needs, service);
            return bindable as TReturn;
        }

        public TReturn Gieven<TService>()
        {
            return Given(container.Type2Service(typeof(TService)));
        }

        public TReturn Given(Func<object> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            bindable.AddContextual(needs, closure);
            return bindable as TReturn;
        }

        internal IGivenData<TReturn> Needs(string needs)
        {
            this.needs = needs;
            return this;
        }
    }
}