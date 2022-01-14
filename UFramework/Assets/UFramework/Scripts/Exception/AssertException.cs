/*
 * @Author: l hy 
 * @Date: 2022-01-14 18:47:22 
 * @Description: 断言异常
 */

using System.Runtime.Serialization;
using SException = System.Exception;
namespace UFramework.Exception {
    public class AssertException : RuntimeException {

        public AssertException () { }

        public AssertException (string message) : base (message) { }

        public AssertException (string message, SException innerException) : base (message, innerException) { }

        public AssertException (SerializationInfo serializationInfo, StreamingContext streamingContext) : base (serializationInfo, streamingContext) { }
    }
}