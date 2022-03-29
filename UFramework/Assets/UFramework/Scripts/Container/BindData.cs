using System;
using UFramework.Exception;
using UFramework.Util;
using UFrameworkContainer = UFramework.Container.Container;

namespace UFramework.Container
{
    public sealed class BindData : IBindData
    {
        public string Service { get; }
        public Func<IContainer, object[], object> Concrete { get; }
        public bool IsStatic { get; }

        public BindData(string service, Func<IContainer, object[], object> concrete, bool isStatic)
        {
            Service = service;
            Concrete = concrete;
            IsStatic = isStatic;
        }
    }
}