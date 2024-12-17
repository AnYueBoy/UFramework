using System;

namespace UFramework
{
    public static class ContainerExtension
    {
        public static IBindData Bind<TService>(this IContainer container)
        {
            return container.Bind(container.Type2Service(typeof(TService)), typeof(TService), false);
        }

        public static IBindData Bind<TService, TConcrete>(this IContainer container)
        {
            return container.Bind(container.Type2Service(typeof(TService)), typeof(TConcrete), false);
        }

        public static IBindData Bind<TService>(this IContainer container, Func<IContainer, object[], object> concrete)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.Bind(container.Type2Service(typeof(TService)), concrete, false);
        }

        public static IBindData Bind<TService>(this IContainer container, Func<object[], object> concrete)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.Bind(container.Type2Service(typeof(TService)), (c, p) => concrete.Invoke(p), false);
        }

        public static IBindData Bind<TService>(this IContainer container, Func<object> concrete)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.Bind(container.Type2Service(typeof(TService)), (c, p) => concrete.Invoke(), false);
        }

        public static IBindData Bind(this IContainer container, string service,
            Func<IContainer, object[], object> concrete)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.Bind(service, concrete, false);
        }

        public static IBindData Singleton(this IContainer container, string service,
            Func<IContainer, object[], object> concrete)
        {
            return container.Bind(service, concrete, true);
        }

        public static IBindData Singleton<TService, TConcrete>(this IContainer container)
        {
            return container.Bind(container.Type2Service(typeof(TService)), typeof(TConcrete), true);
        }

        public static IBindData Singleton<TService>(this IContainer container)
        {
            return container.Bind(container.Type2Service(typeof(TService)), typeof(TService), true);
        }

        public static IBindData Singleton<TService>(
            this IContainer container,
            Func<IContainer, object[], object> concrete)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.Bind(container.Type2Service(typeof(TService)), concrete, true);
        }

        public static IBindData Singleton<TService>(this IContainer container, Func<object[], object> concrete)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.Bind(container.Type2Service(typeof(TService)), (c, p) => concrete.Invoke(p), true);
        }

        public static IBindData Singleton<TService>(
            this IContainer container,
            Func<object> concrete)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.Bind(container.Type2Service(typeof(TService)), (c, p) => concrete.Invoke(), true);
        }

        public static IMethodBind BindMethod(this IContainer container, string method, object target,
            string call = null)
        {
            Guard.ParameterNotNull(method, nameof(method));
            Guard.ParameterNotNull(target, nameof(target));

            return container.BindMethod(method, target, target.GetType().GetMethod(call ?? Str.Method(method)));
        }

        public static IMethodBind BindMethod(this IContainer container, string method, Func<object> callback)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            Guard.Requires<ArgumentNullException>(callback != null);
            return container.BindMethod(method, callback.Target, callback.Method);
        }

        public static IMethodBind BindMethod<T1>(this IContainer container, string method, Func<T1, object> callback)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            Guard.Requires<ArgumentNullException>(callback != null);
            return container.BindMethod(method, callback.Target, callback.Method);
        }

        public static IMethodBind BindMethod<T1, T2>(this IContainer container, string method,
            Func<T1, T2, object> callback)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            Guard.Requires<ArgumentNullException>(callback != null);
            return container.BindMethod(method, callback.Target, callback.Method);
        }

        public static IMethodBind BindMethod<T1, T2, T3>(this IContainer container, string method,
            Func<T1, T2, T3, object> callback)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            Guard.Requires<ArgumentNullException>(callback != null);
            return container.BindMethod(method, callback.Target, callback.Method);
        }

        public static IMethodBind BindMethod<T1, T2, T3, T4>(this IContainer container, string method,
            Func<T1, T2, T3, T4, object> callback)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            Guard.Requires<ArgumentNullException>(callback != null);
            return container.BindMethod(method, callback.Target, callback.Method);
        }

        public static void Unbind<TService>(this IContainer container)
        {
            container.Unbind(container.Type2Service(typeof(TService)));
        }

        public static void Tag<TService>(this IContainer container, string tag)
        {
            container.Tag(tag, container.Type2Service(typeof(TService)));
        }

        public static object Instance<TService>(this IContainer container, object instance)
        {
            return container.Instance(container.Type2Service(typeof(TService)), instance);
        }

        public static bool Release<TService>(this IContainer container)
        {
            return container.Release(container.Type2Service(typeof(TService)));
        }

        public static bool Release(this IContainer container, ref object[] instances, bool reverse = true)
        {
            if (instances == null || instances.Length <= 0)
            {
                return true;
            }

            if (reverse)
            {
                Array.Reverse(instances);
            }

            var errorIndex = 0;

            for (var index = 0; index < instances.Length; index++)
            {
                if (instances[index] == null)
                {
                    continue;
                }

                if (!container.Release(instances[index]))
                {
                    instances[errorIndex++] = instances[index];
                }
            }

            Array.Resize(ref instances, errorIndex);

            if (reverse && errorIndex > 0)
            {
                Array.Reverse(instances);
            }

            return errorIndex <= 0;
        }

        public static void Call<T1>(this IContainer container, Action<T1> method, params object[] userParams)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            container.Call(method.Target, method.Method, userParams);
        }

        public static void Call<T1, T2>(this IContainer container, Action<T1, T2> method, params object[] userParams)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            container.Call(method.Target, method.Method, userParams);
        }

        public static void Call<T1, T2, T3>(this IContainer container, Action<T1, T2, T3> method,
            params object[] userParams)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            container.Call(method.Target, method.Method, userParams);
        }

        public static void Call<T1, T2, T3, T4>(this IContainer container, Action<T1, T2, T3, T4> method,
            params object[] userParams)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            container.Call(method.Target, method.Method, userParams);
        }

        public static object Call(this IContainer container, object target, string method, params object[] userParams)
        {
            Guard.ParameterNotNull(method, nameof(method));
            Guard.ParameterNotNull(target, nameof(target));

            var methodInfo = target.GetType().GetMethod(method);

            if (methodInfo == null)
            {
                throw new LogicException($"Function \"{method}\" not found.");
            }

            return container.Call(target, methodInfo, userParams);
        }

        public static Action Wrap<T1>(this IContainer container, Action<T1> method, params object[] userParams)
        {
            return () =>
            {
                if (method != null)
                {
                    container.Call(method.Target, method.Method, userParams);
                }
            };
        }

        public static Action Wrap<T1, T2>(this IContainer container, Action<T1, T2> method, params object[] userParams)
        {
            return () =>
            {
                if (method != null)
                {
                    container.Call(method.Target, method.Method, userParams);
                }
            };
        }

        public static Action Wrap<T1, T2, T3>(this IContainer container, Action<T1, T2, T3> method,
            params object[] userParams)
        {
            return () =>
            {
                if (method != null)
                {
                    container.Call(method.Target, method.Method, userParams);
                }
            };
        }

        public static Action Wrap<T1, T2, T3, T4>(this IContainer container, Action<T1, T2, T3, T4> method,
            params object[] userParams)
        {
            return () =>
            {
                if (method != null)
                {
                    container.Call(method.Target, method.Method, userParams);
                }
            };
        }

        public static TService Make<TService>(this IContainer container, params object[] userParams)
        {
            return (TService)container.Make(container.Type2Service(typeof(TService)), userParams);
        }

        public static string Type2Service<TService>(this IContainer container)
        {
            return container.Type2Service(typeof(TService));
        }
    }
}