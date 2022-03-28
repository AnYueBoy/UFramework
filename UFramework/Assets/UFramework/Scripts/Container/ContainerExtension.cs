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

        public static TService Make<TService> (this IContainer container, params object[] userParams) {
            return (TService) container.Make (container.Type2Service (typeof (TService)), userParams);
        }

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