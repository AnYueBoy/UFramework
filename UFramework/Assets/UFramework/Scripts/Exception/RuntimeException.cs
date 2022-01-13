/*
 * @Author: l hy 
 * @Date: 2022-01-13 19:33:19 
 * @Description: 断言异常基类 
 */
using System.Runtime.Serialization;
using SException = System.Exception;
namespace UFramework.Exception {
    public class RuntimeException : SException {

        public RuntimeException () { }

        public RuntimeException (string message) : base (message) { }

        public RuntimeException (string message, SException innerException) : base (message, innerException) { }

        public RuntimeException (SerializationInfo serializationInfo, StreamingContext streamingContext) : base (serializationInfo, streamingContext) { }
    }
}