/*
 * @Author: l hy 
 * @Date: 2022-01-14 18:52:36 
 * @Description: 
 */
using SException = System.Exception;
namespace UFramework.Exception {

    /// <summary>
    /// Failed to resolve the service exception.
    /// </summary>
    public class UnresolvableException : RuntimeException {
        public UnresolvableException () { }

        public UnresolvableException (string message) : base (message) { }

        public UnresolvableException (string message, SException innerException) : base (message, innerException) { }
    }
}