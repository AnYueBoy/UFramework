using System;
namespace UFramework.Container {

    /// <summary>
    /// Indicates the given relationship in the context.
    /// </summary>
    public interface IGivenData<TReturn> where TReturn : IBindable {

        /// <summary>
        /// Give the specified service.
        /// </summary>
        TReturn Given (string service);

        TReturn Given<TService> ();

        TReturn Given (Func<object> closure);
    }
}