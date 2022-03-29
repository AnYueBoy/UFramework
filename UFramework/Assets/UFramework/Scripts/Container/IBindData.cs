using System;
namespace UFramework.Container {

    public interface IBindData {

        string Service { get; }

        Func<IContainer, object[], object> Concrete { get; }

        bool IsStatic { get; }
    }
}