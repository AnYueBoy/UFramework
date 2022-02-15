using System;

namespace UFramework.Container {
    public interface IContainer {

        /// <summary>
        /// Resolve the given type from the container.
        /// </summary>
        object this [string service] { get; set; }

        /// <summary>
        /// Gets the binding data of the given service.
        /// </summary>
        IBindData GetBind (string service);

        /// <summary>
        /// Whether the given service has been bind.
        /// </summary>
        bool HasBind (string service);

        /// <summary>
        /// Whether the existing instance is exists in the container.
        /// </summary>
        bool HasInstance (string service);

        /// <summary>
        /// Whether the service has been resolved.
        /// </summary>
        bool IsResolved (string service);

        /// <summary>
        /// Whether the given service can be made.
        /// </summary>
        bool CanMake (string service);

        /// <summary>
        /// Whether the given service is singleton bind. false if the service not exist.
        /// </summary>
        bool IsStatic (string service);

        /// <summary>
        /// Whether the given name is an alias.
        /// </summary>
        bool IsAlias (string name);

        /// <summary>
        /// Register a binding with the container.
        /// </summary>
        IBindData Bind (string service, Type concrete, bool isStatic);

        /// <summary>
        /// Register a binding with the container.
        /// </summary>
        IBindData Bind (string service, Func<IContainer, object[], object> concrete, bool isStatic);

        /// <summary>
        /// Unbinds a service from the container.
        /// </summary>
        void Unbind (string service);

        /// <summary>
        /// Assign a set of tags to a given binding.
        /// </summary>
        void Tag (string tag, params string[] services);

        /// <summary>
        /// Reslove all of the bindings for a given tag.
        /// </summary>
        object[] Tagged (string tag);

        /// <summary>
        /// Register an existing instance as shared in the container.
        /// </summary>
        object Instance (string service, object instance);

        /// <summary>
        /// Release an existing instance in the container.
        /// </summary>
        bool Release (object mixed);

        /// <summary>
        /// Flush the container of all bindings and resolved instances.
        /// </summary>
        void Flush ();

        /// <summary>
        /// Resolve the given service or alias from the container.
        /// </summary>
        object Make (string service, params object[] userParams);

        /// <summary>
        /// Alias a service to a different name.
        /// </summary>
        IContainer Alias (string alias, string service);

        /// <summary>
        /// Register a new resolving callback.
        /// </summary>
        IContainer OnResolving (Action<IBindData, object> closure);

        /// <summary>
        /// Register a new after resolving callback.
        /// </summary>
        IContainer OnAfterResolving (Action<IBindData, object> closure);

        /// <summary>
        /// Register a new release callback.
        /// </summary>
        IContainer OnRelease (Action<IBindData, object> closure);

        /// <summary>
        /// Register a callback for when type finding fails
        /// </summary>
        IContainer OnFindType (Func<string, Type> func, int priority = int.MaxValue);

        /// <summary>
        /// Register a new callback to an abstract's rebind event.
        /// </summary>
        IContainer OnRebound (string service, Action<object> callback);

        /// <summary>
        /// Converts the given type to the service name.
        /// </summary>
        string Type2Service (Type type);
    }
}