using System;
using System.Collections.Generic;
using UContainer = UFramework.Container;

namespace UFramework
{
    public sealed class BindData : Bindable<IBindData>, IBindData
    {
        /// <summary>
        /// resolving 回调列表
        /// </summary>
        private List<Action<IBindData, object>> resolving;

        /// <summary>
        /// resolving 后的回调列表
        /// </summary>
        private List<Action<IBindData, object>> afterResolving;

        /// <summary>
        /// 释放的回调列表
        /// </summary>
        private List<Action<IBindData, object>> release;

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

        public IBindData OnResolving(Action<IBindData, object> closure)
        {
            AddClosure(closure, ref resolving);
            return this;
        }

        public IBindData OnAfterResolving(Action<IBindData, object> closure)
        {
            AddClosure(closure, ref afterResolving);
            return this;
        }

        public IBindData OnRelease(Action<IBindData, object> closure)
        {
            if (!IsStatic)
            {
                throw new LogicException($"服务 [{Service}] 不是静态单例绑定，无法调用释放函数");
            }

            AddClosure(closure, ref release);
            return this;
        }

        internal object TriggerResolving(object instance)
        {
            // 使用容器的触发方法
            return UContainer.Trigger(this, instance, resolving);
        }

        internal object TriggerAfterResolving(object instance)
        {
            return UContainer.Trigger(this, instance, afterResolving);
        }

        internal object TriggerRelease(object instance)
        {
            return UContainer.Trigger(this, instance, release);
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