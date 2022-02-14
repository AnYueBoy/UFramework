using System;
namespace UFramework.Container {

    public interface IBindData {

        /// <summary>
        /// Gets the service name.
        /// </summary>
        string Service { get; }

        /// <summary>
        /// Gets the container to which the service belongs.
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// Gets the delegate return service concrete.
        /// </summary>
        Func<IContainer, object[], object> Concrete { get; }

        /// <summary>
        /// Gets a value indicating whether true if the service is singleton(static).
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        ///  Unbind the service from the container.
        /// </summary>
        /// <remarks>
        /// If the service is a singletoned instance, then the singleton instance
        /// that has been built will be automatically released.
        /// </remarks>
        void Unbind ();

        /// <summary>
        /// Alias service to a different name.
        /// </summary>
        IBindData Alias (string alias);

        IBindData Alias<TAlias> ();
    }
}