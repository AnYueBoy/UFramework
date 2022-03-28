using System;

namespace UFramework.Container {
    public interface IContainer {

        IBindData GetBind (string service);

        bool HasBind (string service);

        bool HasInstance (string service);

        bool IsResolved (string service);

        bool CanMake (string service);

        bool IsStatic (string service);

        IBindData Bind (string service, Type concrete, bool isStatic);

        IBindData Bind (string service, Func<IContainer, object[], object> concrete, bool isStatic);

        void Unbind (string service);

        object Instance (string service, object instance);

        bool Release (object mixed);

        void Flush ();
        
        IContainer OnFindType (Func<string, Type> func, int priority = int.MaxValue);

        object Make (string service, params object[] userParams);
       
        IContainer OnResolving (Action<IBindData, object> closure);

        IContainer OnAfterResolving (Action<IBindData, object> closure);

        IContainer OnRelease (Action<IBindData, object> closure);

        string Type2Service (Type type);
    }
}