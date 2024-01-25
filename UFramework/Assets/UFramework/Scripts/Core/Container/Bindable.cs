using System;
using System.Collections.Generic;

namespace UFramework
{
    public abstract class Bindable : IBindable
    {
        private readonly Container container;
        private Dictionary<string, string> contextual;
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

        /// <summary>
        /// 添加服务的上下文
        /// </summary>
        /// <param name="needs">服务所关联的上下文</param>
        /// <param name="given">给定的服务或别名</param>
        internal void AddContextual(string needs, string given)
        {
            AssertDestroyed();
            if (contextual == null)
            {
                contextual = new Dictionary<string, string>();
            }

            if (contextual.ContainsKey(needs) || !(contextualClosure != null && contextualClosure.ContainsKey(needs)))
            {
                throw new LogicException($"{needs} 已经存在");
            }

            contextual.Add(needs, given);
        }

        internal void AddContextual(string needs, Func<object> given)
        {
            AssertDestroyed();
            if (contextualClosure == null)
            {
                contextualClosure = new Dictionary<string, Func<object>>();
            }

            if (contextualClosure.ContainsKey(needs) || (contextual != null && contextual.ContainsKey(needs)))
            {
                throw new LogicException($"{needs} 已存在");
            }

            contextualClosure.Add(needs, given);
        }

        /// <summary>
        /// 获取服务需要的上下文
        /// </summary>
        internal string GetContextual(string needs)
        {
            if (contextual == null)
            {
                return null;
            }

            return contextual.TryGetValue(needs, out string contextualNeeds) ? contextualNeeds : null;
        }

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

    public abstract class Bindable<TReturn> : Bindable, IBindable<TReturn> where TReturn : class, IBindable<TReturn>
    {
        /// <summary>
        /// 包含服务与上下文关系的数据
        /// </summary>
        private GivenData<TReturn> given;

        protected Bindable(Container container, string service) : base(container, service)
        {
        }

        public IGivenData<TReturn> Needs(string service)
        {
            Guard.ParameterNotNull(service, nameof(service));

            AssertDestroyed();

            if (given == null)
            {
                given = new GivenData<TReturn>((Container)Container, this);
            }

            given.Needs(service);
            return given;
        }

        public IGivenData<TReturn> Needs<TService>()
        {
            return Needs(Container.Type2Service(typeof(TService)));
        }
    }
}