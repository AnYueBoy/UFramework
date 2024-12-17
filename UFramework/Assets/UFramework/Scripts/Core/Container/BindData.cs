using System;

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
    }
}