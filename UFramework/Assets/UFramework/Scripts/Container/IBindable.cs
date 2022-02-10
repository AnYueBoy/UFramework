/*
 * @Author: l hy 
 * @Date: 2022-01-15 09:39:32 
 */

namespace UFramework.Container {
    public interface IBindable {

        /// <summary>
        /// Gets the service name.
        /// </summary>
        string Service { get; }

        /// <summary>
        /// Gets the container to which the service belongs.
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        ///  Unbind the service from the container.
        /// </summary>
        /// <remarks>
        /// If the service is a singletoned instance, then the singleton instance
        /// that has been built will be automatically released.
        /// </remarks>
        void Unbind ();
    }

    public interface IBindable<TReturn> : IBindable where TReturn : IBindable {
      
    }
}