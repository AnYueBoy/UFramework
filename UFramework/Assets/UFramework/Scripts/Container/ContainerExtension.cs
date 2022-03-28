using System;
using UFramework.Util;

namespace UFramework.Container {

    public static class ContainerExtension {

        public static IBindData GetBind<TService> (this IContainer container) {
            return container.GetBind (container.Type2Service (typeof (TService)));
        }

        public static bool HasBind<TService> (this IContainer container) {
            return container.HasBind (container.Type2Service (typeof (TService)));
        }

        public static bool HasInstance<TService> (this IContainer container) {
            return container.HasInstance (container.Type2Service (typeof (TService)));
        }

        public static bool IsResolved<TService> (this IContainer container) {
            return container.IsResolved (container.Type2Service (typeof (TService)));
        }

        public static bool CanMake<TService> (this IContainer container) {
            return container.CanMake (container.Type2Service (typeof (TService)));
        }

        public static bool IsStatic<TService> (this IContainer container) {
            return container.IsStatic (container.Type2Service (typeof (TService)));
        }
        
        public static IBindData Singleton<TService, TConcrete> (this IContainer container) {
            return container.Bind (container.Type2Service (typeof (TService)), typeof (TConcrete), true);
        }

        public static IBindData Singleton<TService> (this IContainer container) {
            return container.Bind (container.Type2Service (typeof (TService)), typeof (TService), true);
        }

        public static IBindData Singleton<TService> (
            this IContainer container,
            Func<object> concrete) {
            Guard.Requires<ArgumentNullException> (concrete != null);
            return container.Bind (container.Type2Service (typeof (TService)), (c, p) => concrete.Invoke (), true);
        }

        public static void Unbind<TService> (this IContainer container) {
            container.Unbind (container.Type2Service (typeof (TService)));
        }

        public static object Instance<TService> (this IContainer container, object instance) {
            return container.Instance (container.Type2Service (typeof (TService)), instance);
        }

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
        
        public static string Type2Service<TService> (this IContainer container) {
            return container.Type2Service (typeof (TService));
        }

        public static Func<TService> Factory<TService> (this IContainer container, params object[] userParams) {
            return () => (TService) container.Make (container.Type2Service (typeof (TService)), userParams);
        }
        public static Func<object> Factory (this IContainer container, string service, params object[] userParams) {
            return () => container.Make (service, userParams);
        }
    }
}