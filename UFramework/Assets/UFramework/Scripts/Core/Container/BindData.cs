using System;
using System.Collections.Generic;
using UContainer = UFramework.Container;

namespace UFramework
{
    public sealed class BindData : Bindable, IBindData
    {
        public BindData(Container container, string service, Func<IContainer, object[], object> concrete, bool isStatic)
            : base(container, service)
        {
            Concrete = concrete;
            IsStatic = isStatic;
        }

        public Func<IContainer, object[], object> Concrete { get; }
        public bool IsStatic { get; }

        public IBindData Tag(string tag)
        {
            AssertDestroyed();
            Guard.ParameterNotNull(tag, nameof(tag));
            Container.Tag(tag, Service);
            return this;
        }

        protected override void ReleaseBind()
        {
            ((Container)Container).Unbind(this);
        }

        /// <summary>
        /// 向指定的回调列表中添加回调
        /// </summary>
        private void AddClosure(Action<IBindData, object> closure, ref List<Action<IBindData, object>> collection)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            AssertDestroyed();
            if (collection == null)
            {
                collection = new List<Action<IBindData, object>>();
            }

            collection.Add(closure);
        }
    }
}