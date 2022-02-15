using System;
using UFramework.Exception;
using UFramework.Util;

namespace UFramework.Container {
    /// <summary>
    /// An extension function for <see cref="Container"/>.
    /// </summary>
    public static class ContainerExtension {
        /// <summary>
        /// Gets the binding data of the given service.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>Return null If there is no binding data.</returns>
        public static IBindData GetBind<TService> (this IContainer container) {
            return container.GetBind (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Whether the given service has been bound.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>True if the service has been bound.</returns>
        public static bool HasBind<TService> (this IContainer container) {
            return container.HasBind (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Whether the existing instance is exists in the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>True if the instance existed.</returns>
        public static bool HasInstance<TService> (this IContainer container) {
            return container.HasInstance (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Whether the service has been resolved.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>True if the service has been resolved.</returns>
        public static bool IsResolved<TService> (this IContainer container) {
            return container.IsResolved (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Whether the given service can be made.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>True if the given service can be made.</returns>
        public static bool CanMake<TService> (this IContainer container) {
            return container.CanMake (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Whether the given service is singleton bind. false if the service not exists.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>True if the service is singleton bind.</returns>
        public static bool IsStatic<TService> (this IContainer container) {
            return container.IsStatic (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Whether the given name is an alias.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>True if the given name is an alias.</returns>
        public static bool IsAlias<TService> (this IContainer container) {
            return container.IsAlias (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Alias a service to a different name.
        /// </summary>
        /// <typeparam name="TAlias">The alias name.</typeparam>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The container instance.</param>
        /// <returns>Returns the container instance.</returns>
        public static IContainer Alias<TAlias, TService> (this IContainer container) {
            return container.Alias (container.Type2Service (typeof (TAlias)), container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Register a binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name (also indicates specific implementation).</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Bind<TService> (this IContainer container) {
            return container.Bind (container.Type2Service (typeof (TService)), typeof (TService), false);
        }

        /// <summary>
        /// Register a binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <typeparam name="TConcrete">The service concrete.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Bind<TService, TConcrete> (this IContainer container) {
            return container.Bind (container.Type2Service (typeof (TService)), typeof (TConcrete), false);
        }

        /// <summary>
        /// Register a binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="concrete">The service concrete.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Bind<TService> (this IContainer container, Func<IContainer, object[], object> concrete) {
            Guard.Requires<ArgumentNullException> (concrete != null);
            return container.Bind (container.Type2Service (typeof (TService)), concrete, false);
        }

        /// <summary>
        /// Register a binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="concrete">The service concrete.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Bind<TService> (this IContainer container, Func<object[], object> concrete) {
            Guard.Requires<ArgumentNullException> (concrete != null);
            return container.Bind (container.Type2Service (typeof (TService)), (c, p) => concrete.Invoke (p), false);
        }

        /// <summary>
        /// Register a binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="concrete">The service concrete.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Bind<TService> (this IContainer container, Func<object> concrete) {
            Guard.Requires<ArgumentNullException> (concrete != null);
            return container.Bind (container.Type2Service (typeof (TService)), (c, p) => concrete.Invoke (), false);
        }

        /// <summary>
        /// Register a binding with the container.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="service">The service name.</param>
        /// <param name="concrete">The service concrete.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Bind (this IContainer container, string service,
            Func<IContainer, object[], object> concrete) {
            Guard.Requires<ArgumentNullException> (concrete != null);
            return container.Bind (service, concrete, false);
        }

        /// <summary>
        /// Register a singleton binding with the container.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="service">The service name.</param>
        /// <param name="concrete">The service concrete.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Singleton (this IContainer container, string service,
            Func<IContainer, object[], object> concrete) {
            return container.Bind (service, concrete, true);
        }

        /// <summary>
        /// Register a singleton binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <typeparam name="TConcrete">The service concrete.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Singleton<TService, TConcrete> (this IContainer container) {
            return container.Bind (container.Type2Service (typeof (TService)), typeof (TConcrete), true);
        }

        /// <summary>
        /// Register a singleton binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name (also indicates specific implementation).</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Singleton<TService> (this IContainer container) {
            return container.Bind (container.Type2Service (typeof (TService)), typeof (TService), true);
        }

        /// <summary>
        /// Register a singleton binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="concrete">The service concrete.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Singleton<TService> (
            this IContainer container,
            Func<IContainer, object[], object> concrete) {
            Guard.Requires<ArgumentNullException> (concrete != null);
            return container.Bind (container.Type2Service (typeof (TService)), concrete, true);
        }

        /// <summary>
        /// Register a singleton binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="concrete">The service concrete.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Singleton<TService> (this IContainer container, Func<object[], object> concrete) {
            Guard.Requires<ArgumentNullException> (concrete != null);
            return container.Bind (container.Type2Service (typeof (TService)), (c, p) => concrete.Invoke (p), true);
        }

        /// <summary>
        /// Register a singleton binding with the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="concrete">The service concrete.</param>
        /// <returns>The service binding data.</returns>
        public static IBindData Singleton<TService> (
            this IContainer container,
            Func<object> concrete) {
            Guard.Requires<ArgumentNullException> (concrete != null);
            return container.Bind (container.Type2Service (typeof (TService)), (c, p) => concrete.Invoke (), true);
        }

        /// <summary>
        /// Unbinds a service from the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        public static void Unbind<TService> (this IContainer container) {
            container.Unbind (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Assign a set of tags to a given binding.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="tag">The tag name.</param>
        public static void Tag<TService> (this IContainer container, string tag) {
            container.Tag (tag, container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Register an existing instance as shared in the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="instance">The service instance.</param>
        /// <returns>Object processed by the decorator.</returns>
        public static object Instance<TService> (this IContainer container, object instance) {
            return container.Instance (container.Type2Service (typeof (TService)), instance);
        }

        /// <summary>
        /// Release an existing instance in the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>True if the instance is released. otherwise if instance not exits return false.</returns>
        public static bool Release<TService> (this IContainer container) {
            return container.Release (container.Type2Service (typeof (TService)));
        }

        /// <summary>
        /// Release an existing instance in the container.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="instances">Service instance that needs to be released.</param>
        /// <param name="reverse">Whether to reverse the release order.</param>
        /// <returns>Returns false if one has not been successfully released, <paramref name="instances"/> is an instance that has not been released.</returns>
        public static bool Release (this IContainer container, ref object[] instances, bool reverse = true) {
            if (instances == null || instances.Length <= 0) {
                return true;
            }

            if (reverse) {
                Array.Reverse (instances);
            }

            var errorIndex = 0;

            for (var index = 0; index < instances.Length; index++) {
                if (instances[index] == null) {
                    continue;
                }

                if (!container.Release (instances[index])) {
                    instances[errorIndex++] = instances[index];
                }
            }

            Array.Resize (ref instances, errorIndex);

            if (reverse && errorIndex > 0) {
                Array.Reverse (instances);
            }

            return errorIndex <= 0;
        }

        /// <summary>
        /// Resolve the given service or alias from the container.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="userParams">The user parameters.</param>
        /// <returns>The serivce instance. Throw exception if the service can not resolved.</returns>
        public static TService Make<TService> (this IContainer container, params object[] userParams) {
            return (TService) container.Make (container.Type2Service (typeof (TService)), userParams);
        }

        /// <summary>
        /// Resolve the given service or alias from the container.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="type">The service type.</param>
        /// <param name="userParams">The user parameters.</param>
        /// <returns>The serivce instance. Throw exception if the service can not resolved.</returns>
        public static object Make (this IContainer container, Type type, params object[] userParams) {
            var service = container.Type2Service (type);
            container.Bind (service, type, false);
            return container.Make (service, userParams);
        }

        /// <summary>
        /// Register a new release callback.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnRelease (this IContainer container, Action<object> callback) {
            Guard.Requires<ArgumentNullException> (callback != null);
            return container.OnRelease ((_, instance) => callback (instance));
        }

        /// <summary>
        /// Register a new release callback.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="closure">The callback.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnRelease<T> (this IContainer container, Action<T> closure) {
            Guard.Requires<ArgumentNullException> (closure != null);
            return container.OnRelease ((_, instance) => {
                if (instance is T) {
                    closure ((T) instance);
                }
            });
        }

        /// <summary>
        /// Register a new release callback.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="closure">The callback.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnRelease<T> (this IContainer container, Action<IBindData, T> closure) {
            Guard.Requires<ArgumentNullException> (closure != null);
            return container.OnRelease ((bindData, instance) => {
                if (instance is T) {
                    closure (bindData, (T) instance);
                }
            });
        }

        /// <summary>
        /// Register a new resolving callback.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnResolving (this IContainer container, Action<object> callback) {
            Guard.Requires<ArgumentNullException> (callback != null);
            return container.OnResolving ((_, instance) => {
                callback (instance);
            });
        }

        /// <summary>
        /// Register a new resolving callback.
        /// <para>Only the type matches the given type will be called back.</para>
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="closure">The closure.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnResolving<T> (this IContainer container, Action<T> closure) {
            Guard.Requires<ArgumentNullException> (closure != null);
            return container.OnResolving ((_, instance) => {
                if (instance is T) {
                    closure ((T) instance);
                }
            });
        }

        /// <summary>
        /// Register a new resolving callback.
        /// <para>Only the type matches the given type will be called back.</para>
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="closure">The closure.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnResolving<T> (this IContainer container, Action<IBindData, T> closure) {
            Guard.Requires<ArgumentNullException> (closure != null);
            return container.OnResolving ((bindData, instance) => {
                if (instance is T) {
                    closure (bindData, (T) instance);
                }
            });
        }

        /// <summary>
        /// Register a new after resolving callback.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnAfterResolving (this IContainer container, Action<object> callback) {
            Guard.Requires<ArgumentNullException> (callback != null);
            return container.OnAfterResolving ((_, instance) => {
                callback (instance);
            });
        }

        /// <summary>
        /// Register a new after resolving callback.
        /// <para>Only the type matches the given type will be called back.</para>
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="closure">The closure.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnAfterResolving<T> (this IContainer container, Action<T> closure) {
            Guard.Requires<ArgumentNullException> (closure != null);
            return container.OnAfterResolving ((_, instance) => {
                if (instance is T) {
                    closure ((T) instance);
                }
            });
        }

        /// <summary>
        /// Register a new after resolving callback.
        /// <para>Only the type matches the given type will be called back.</para>
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="closure">The closure.</param>
        /// <returns>Returns the <see cref="IContainer"/> instance.</returns>
        public static IContainer OnAfterResolving<T> (this IContainer container, Action<IBindData, T> closure) {
            Guard.Requires<ArgumentNullException> (closure != null);
            return container.OnAfterResolving ((bindData, instance) => {
                if (instance is T) {
                    closure (bindData, (T) instance);
                }
            });
        }

        /// <summary>
        /// Watch the specified service, trigger callback when rebinding the service.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="method">The callback.</param>
        public static void Watch<TService> (this IContainer container, Action method) {
            Guard.Requires<ArgumentNullException> (method != null);
            container.OnRebound (container.Type2Service (typeof (TService)), (instance) => method ());
        }

        /// <summary>
        /// Watch the specified service, trigger callback when rebinding the service.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="method">The callback.</param>
        public static void Watch<TService> (this IContainer container, Action<TService> method) {
            Guard.Requires<ArgumentNullException> (method != null);
            container.OnRebound (container.Type2Service (typeof (TService)), (instance) => method ((TService) instance));
        }

        /// <summary>
        /// Converts the given type to the service name.
        /// </summary>
        /// <typeparam name="TService">The given type.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <returns>The service name.</returns>
        public static string Type2Service<TService> (this IContainer container) {
            return container.Type2Service (typeof (TService));
        }

        /// <summary>
        /// Lazially resolve a service that is built when the call returns a callback.
        /// </summary>
        /// <typeparam name="TService">The service name.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="userParams">The user parameters.</param>
        /// <returns>The callback.</returns>
        public static Func<TService> Factory<TService> (this IContainer container, params object[] userParams) {
            return () => (TService) container.Make (container.Type2Service (typeof (TService)), userParams);
        }

        /// <summary>
        /// Lazially resolve a service that is built when the call returns a callback.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance.</param>
        /// <param name="service">The service name or alias.</param>
        /// <param name="userParams">The user parameters.</param>
        /// <returns>The callback.</returns>
        public static Func<object> Factory (this IContainer container, string service, params object[] userParams) {
            return () => container.Make (service, userParams);
        }
    }
}