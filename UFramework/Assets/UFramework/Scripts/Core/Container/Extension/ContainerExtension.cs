using System;

namespace UFramework
{
    public static class ContainerExtension
    {
        public static IBindData GetBind<TService>(this IContainer container)
        {
            return container.GetBind(container.Type2Service(typeof(TService)));
        }

        public static bool HasBind<TService>(this IContainer container)
        {
            return container.HasBind(container.Type2Service(typeof(TService)));
        }

        public static bool HasInstance<TService>(this IContainer container)
        {
            return container.HasInstance(container.Type2Service(typeof(TService)));
        }

        public static bool IsResolved<TService>(this IContainer container)
        {
            return container.IsResolved(container.Type2Service(typeof(TService)));
        }

        public static bool CanMake<TService>(this IContainer container)
        {
            return container.CanMake(container.Type2Service(typeof(TService)));
        }

        public static bool IsStatic<TService>(this IContainer container)
        {
            return container.IsStatic(container.Type2Service(typeof(TService)));
        }

        public static bool IsAlias<TService>(this IContainer container)
        {
            return container.IsAlias(container.Type2Service(typeof(TService)));
        }

        public static IContainer Alias<TAlias, TService>(this IContainer container)
        {
            return container.Alias(container.Type2Service(typeof(TAlias)), container.Type2Service(typeof(TService)));
        }

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

        public static bool BindIf<TService, TConcrete>(this IContainer container, out IBindData bindData)
        {
            return container.BindIf(container.Type2Service(typeof(TService)), typeof(TConcrete), false, out bindData);
        }

        public static bool BindIf<TService>(this IContainer container, out IBindData bindData)
        {
            return container.BindIf(container.Type2Service(typeof(TService)), typeof(TService), false, out bindData);
        }

        public static bool BindIf<TService>(this IContainer container, Func<IContainer, object[], object> concrete,
            out IBindData bindData)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.BindIf(container.Type2Service(typeof(TService)), concrete, false, out bindData);
        }

        public static bool BindIf<TService>(this IContainer container, Func<object[], object> concrete,
            out IBindData bindData)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.BindIf(container.Type2Service(typeof(TService)), (c, @params) => concrete(@params), false,
                out bindData);
        }

        public static bool BindIf<TService>(this IContainer container, Func<object> concrete, out IBindData bindData)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.BindIf(container.Type2Service(typeof(TService)), (c, p) => concrete.Invoke(), false,
                out bindData);
        }

        public static bool BindIf(this IContainer container, string service,
            Func<IContainer, object[], object> concrete, out IBindData bindData)
        {
            return container.BindIf(service, concrete, false, out bindData);
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

        public static bool SingletonIf<TService, TConcrete>(this IContainer container, out IBindData bindData)
        {
            return container.BindIf(container.Type2Service(typeof(TService)), typeof(TConcrete), true, out bindData);
        }

        public static bool SingletonIf<TService>(this IContainer container, out IBindData bindData)
        {
            return container.BindIf(container.Type2Service(typeof(TService)), typeof(TService), true, out bindData);
        }

        public static bool SingletonIf<TService>(this IContainer container, Func<IContainer, object[], object> concrete,
            out IBindData bindData)
        {
            return container.BindIf(container.Type2Service(typeof(TService)), concrete, true, out bindData);
        }

        public static bool SingletonIf<TService>(this IContainer container, Func<object> concrete,
            out IBindData bindData)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.BindIf(container.Type2Service(typeof(TService)), (c, p) => concrete.Invoke(), true,
                out bindData);
        }

        public static bool SingletonIf<TService>(this IContainer container, Func<object[], object> concrete,
            out IBindData bindData)
        {
            Guard.Requires<ArgumentNullException>(concrete != null);
            return container.BindIf(container.Type2Service(typeof(TService)), (c, @params) => concrete(@params), true,
                out bindData);
        }

        public static bool SingletonIf(this IContainer container, string service,
            Func<IContainer, object[], object> concrete, out IBindData bindData)
        {
            return container.BindIf(service, concrete, true, out bindData);
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

        public static object Make(this IContainer container, Type type, params object[] userParams)
        {
            var service = container.Type2Service(type);
            container.BindIf(service, type, false, out _);
            return container.Make(service, userParams);
        }

        public static void Extend(this IContainer container, string service, Func<object, object> closure)
        {
            container.Extend(service, (instance, c) => closure(instance));
        }

        public static void Extend<TService, TConcrete>(this IContainer container, Func<TConcrete, object> closure)
        {
            container.Extend(container.Type2Service(typeof(TService)), (instance, c) => closure((TConcrete)instance));
        }

        public static void Extend<TService, TConcrete>(this IContainer container,
            Func<TConcrete, IContainer, object> closure)
        {
            container.Extend(
                container.Type2Service(typeof(TService)),
                (instance, c) => closure((TConcrete)instance, c));
        }

        public static void Extend<TConcrete>(this IContainer container, Func<TConcrete, IContainer, object> closure)
        {
            container.Extend(null, (instance, c) =>
            {
                if (instance is TConcrete)
                {
                    return closure((TConcrete)instance, c);
                }

                return instance;
            });
        }

        public static void Extend<TConcrete>(this IContainer container, Func<TConcrete, object> closure)
        {
            container.Extend(null, (instance, _) =>
            {
                if (instance is TConcrete)
                {
                    return closure((TConcrete)instance);
                }

                return instance;
            });
        }

        public static IContainer OnRelease(this IContainer container, Action<object> callback)
        {
            Guard.Requires<ArgumentNullException>(callback != null);
            return container.OnRelease((_, instance) => callback(instance));
        }

        public static IContainer OnRelease<T>(this IContainer container, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return container.OnRelease((_, instance) =>
            {
                if (instance is T)
                {
                    closure((T)instance);
                }
            });
        }

        public static IContainer OnRelease<T>(this IContainer container, Action<IBindData, T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return container.OnRelease((bindData, instance) =>
            {
                if (instance is T)
                {
                    closure(bindData, (T)instance);
                }
            });
        }

        public static IContainer OnResolving(this IContainer container, Action<object> callback)
        {
            Guard.Requires<ArgumentNullException>(callback != null);
            return container.OnResolving((_, instance) => { callback(instance); });
        }

        public static IContainer OnResolving<T>(this IContainer container, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return container.OnResolving((_, instance) =>
            {
                if (instance is T)
                {
                    closure((T)instance);
                }
            });
        }

        public static IContainer OnResolving<T>(this IContainer container, Action<IBindData, T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return container.OnResolving((bindData, instance) =>
            {
                if (instance is T)
                {
                    closure(bindData, (T)instance);
                }
            });
        }

        public static IContainer OnAfterResolving(this IContainer container, Action<object> callback)
        {
            Guard.Requires<ArgumentNullException>(callback != null);
            return container.OnAfterResolving((_, instance) => { callback(instance); });
        }

        public static IContainer OnAfterResolving<T>(this IContainer container, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return container.OnAfterResolving((_, instance) =>
            {
                if (instance is T)
                {
                    closure((T)instance);
                }
            });
        }

        public static IContainer OnAfterResolving<T>(this IContainer container, Action<IBindData, T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return container.OnAfterResolving((bindData, instance) =>
            {
                if (instance is T)
                {
                    closure(bindData, (T)instance);
                }
            });
        }

        public static void Watch<TService>(this IContainer container, Action method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            container.OnRebound(container.Type2Service(typeof(TService)), (instance) => method());
        }

        public static void Watch<TService>(this IContainer container, Action<TService> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            container.OnRebound(container.Type2Service(typeof(TService)), (instance) => method((TService)instance));
        }

        public static string Type2Service<TService>(this IContainer container)
        {
            return container.Type2Service(typeof(TService));
        }

        public static Func<TService> Factory<TService>(this IContainer container, params object[] userParams)
        {
            return () => (TService)container.Make(container.Type2Service(typeof(TService)), userParams);
        }

        public static Func<object> Factory(this IContainer container, string service, params object[] userParams)
        {
            return () => container.Make(service, userParams);
        }
    }
}