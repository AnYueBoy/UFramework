using System;
namespace UFramework.Container {

    public interface IBindData : IBindable {

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

        IBindData Alias<TAlias> ();

        /// <summary>
        /// Assign a tag to a given service.
        /// </summary>
        IBindData Tag (string tag);
    }
}