/*
 * @Author: l hy 
 * @Date: 2022-01-13 16:55:30 
 * @Description: 守卫程序，用简单优雅的代码写出断言代码
 */
using System.Reflection;
using SException = System.Exception;
using System;
using System.Collections.Generic;
namespace UFramework.Util {
    public sealed class Guard {

        private static Guard that;

        private static IDictionary<Type, ExtendException> exceptionFactory;

        public delegate SException ExtendException (string message, SException innerException, object state);

        public static Guard That {
            get {
                if (that == null) {
                    that = new Guard ();
                }
                return that;
            }
        }

        public static void Requires<TException> (
            bool condition,
            string message = null,
            SException innerException = null,
            object state = null) where TException : SException, new () {
            Requires (typeof (TException), condition, message, innerException, state);
        }

        public static void Requires (
            Type exception,
            bool condition,
            string message = null,
            SException innerException = null,
            object state = null) {

            if (condition) {
                return;
            }

            throw CreateExceptionInstance (exception, message, innerException, state);
        }

        /// <summary>
        /// The verification parameter is not Null.
        /// </summary>
        public static void ParameterNotNull (
            object argumentValue,
            string argumentName,
            string message = null,
            SException innerException = null) {

            if (argumentValue != null) {
                return;
            }

            message = message??$"Parameter {argumentName} not allowed for null. please check the function input.";
            ArgumentNullException exception = new ArgumentNullException (argumentName, message);

            if (innerException != null) {
                SetField (exception, "_innerException", innerException);
            }

            throw exception;
        }

        private static SException CreateExceptionInstance (
            Type exceptionType,
            string message,
            SException innerException,
            object state) {

            if (!typeof (SException).IsAssignableFrom (exceptionType)) {
                // ArgumentException 参数异常错误 nameof 用来获取成员名称
                throw new ArgumentException (
                    $"Type: {exceptionType} must be inherited from: {typeof(SException)}.",
                    nameof (exceptionType));
            }

            VerifyExceptionFactory ();

            if (exceptionFactory.TryGetValue (exceptionType, out ExtendException factory)) {
                SException ret = factory (message, innerException, state);
                if (ret != null) {
                    return ret;
                }
            }

            // 通过反射创建类的实例
            object exception = Activator.CreateInstance (exceptionType);
            if (!string.IsNullOrEmpty (message)) {
                SetField (exception, "_message", message);
            }

            if (innerException != null) {
                SetField (exception, "_innerException", innerException);
            }

            return (SException) exception;
        }

        private static void VerifyExceptionFactory () {
            if (exceptionFactory == null) {
                exceptionFactory = new Dictionary<Type, ExtendException> ();
            }
        }

        private static void SetField (object obj, string filed, object value) {
            // 标记用于反射的时候查找类型成员
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo filedInfo = obj.GetType ().GetField (filed, flag);

            if (filedInfo != null) {
                // 设置反射成员变量值
                filedInfo.SetValue (obj, value);
            }
        }
    }

}