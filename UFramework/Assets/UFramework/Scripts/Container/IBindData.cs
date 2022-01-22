using System;
namespace UFramework.Container {

    public interface IBindData : IBindable<IBindData> {

        /// <summary>
        /// Gets the delegate return service concrete.
        /// </summary>
        Func<IContainer, object[], object> Concrete { get; }

        /// <summary>
        /// Gets a value indicating whether true if the service is singleton(static).
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// Alias service to a different name.
        /// </summary>
        IBindData Alias (string alias);

        /// <summary>
        /// Assign a tag to a given service.
        /// </summary>
        IBindData Tag (string tag);

        /// <summary>
        /// Register a new resolving callback.
        /// </summary>
        IBindData OnResolving (Action<IBindData, object> closure);

        /// <summary>
        /// Register a new after resolving callback.
        /// </summary>
        IBindData OnAfterResolving (Action<IBindData, object> closure);

        /// <summary>
        /// Register a new release callback.
        /// </summary>
        IBindData OnRelease (Action<IBindData, object> closure);
    }
}