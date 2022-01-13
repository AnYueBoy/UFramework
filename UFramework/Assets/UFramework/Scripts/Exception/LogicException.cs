/*
 * @Author: l hy 
 * @Date: 2022-01-13 19:43:22 
 * @Description: 逻辑断言
 */
using System.Runtime.Serialization;
using SException = System.Exception;
namespace UFramework.Exception {
    public class LogicException : RuntimeException {

        public LogicException () { }

        public LogicException (string message) : base (message) { }

        public LogicException (string message, SException innerException) : base (message, innerException) { }

        public LogicException (SerializationInfo serializationInfo, StreamingContext streamingContext) : base (serializationInfo, streamingContext) { }
    }
}