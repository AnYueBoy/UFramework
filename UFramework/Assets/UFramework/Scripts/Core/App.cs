using System;
using UFramework.Container;

namespace UFramework.Core {
    public abstract class App {
        /// <summary>
        /// The IApplication instance.
        /// </summary>
        private static IApplication that;

        /// <summary>
        /// Callback when a new IApplication instance is created.
        /// </summary>
        public static event Action<IApplication> OnNewApplication {
            add {
                RaiseOnNewApplication += value;
                if (That != null) {
                    value?.Invoke (That);
                }
            }
            remove => RaiseOnNewApplication -= value;
        }

        /// <summary>
        /// Callback when a new IApplication instance is created.
        /// </summary>
        private static event Action<IApplication> RaiseOnNewApplication;

        /// <summary>
        /// Gets or Sets the IApplication instance.
        /// </summary>
        public static IApplication That {
            get {
                return that;
            }

            set {
                that = value;
                RaiseOnNewApplication?.Invoke (that);
            }
        }

        public static bool IsMainThread => That.IsMainThread;

        public static DebugLevel DebugLevel {
            get => That.DebugLevel;
            set => That.DebugLevel = value;
        }

        public static void Terminate () {
            That.Terminate ();
        }

        public static void Register (IServiceProvider provider, bool force = false) {
            That.Register (provider, force);
        }

        public static bool IsRegistered (IServiceProvider provider) {
            return That.IsRegistered (provider);
        }

        public static long GetRuntimeId () {
            return That.GetRuntimeId ();
        }

        public static IContainer OnFindType (Func<string, Type> func, int priority = int.MaxValue) {
            return That.OnFindType (func, priority);
        }

        public static IContainer OnRebound (string service, Action<object> callback) {
            return That.OnRebound (service, callback);
        }

        public static IBindData GetBind (string service) {
            return That.GetBind (service);
        }

        public static IBindData GetBind<TService> () {
            return That.GetBind<TService> ();
        }

        public static bool HasInstance (string service) {
            return That.HasInstance (service);
        }

        public static bool HasInstance<TService> () {
            return That.HasInstance<TService> ();
        }

        public static bool IsResolved (string service) {
            return That.IsResolved (service);
        }

        public static bool IsResolved<TService> () {
            return That.IsResolved<TService> ();
        }

        public static bool HasBind (string service) {
            return That.HasBind (service);
        }

        public static bool HasBind<TService> () {
            return That.HasBind<TService> ();
        }

        public static bool CanMake (string service) {
            return That.CanMake (service);
        }

        public static bool CanMake<TService> () {
            return That.CanMake<TService> ();
        }

        public static bool IsStatic (string service) {
            return That.IsStatic (service);
        }

        public static bool IsStatic<TService> () {
            return That.IsStatic<TService> ();
        }

        public static bool IsAlias (string name) {
            return That.IsAlias (name);
        }

        public static bool IsAlias<TService> () {
            return That.IsAlias<TService> ();
        }

        public static IContainer Alias (string alias, string service) {
            return That.Alias (alias, service);
        }

        public static IContainer Alias<TAlias, TService> () {
            return That.Alias<TAlias, TService> ();
        }

        public static IBindData Bind<TService> () {
            return That.Bind<TService> ();
        }

        public static IBindData Bind<TService, TConcrete> () {
            return That.Bind<TService, TConcrete> ();
        }

        public static IBindData Bind (string service, Type concrete, bool isStatic) {
            return That.Bind (service, concrete, isStatic);
        }

        public static IBindData Bind (string service, Func<IContainer, object[], object> concrete, bool isStatic) {
            return That.Bind (service, concrete, isStatic);
        }

        public static IBindData Bind<TService> (Func<IContainer, object[], object> concrete) {
            return That.Bind<TService> (concrete);
        }

        public static IBindData Bind<TService> (Func<object[], object> concrete) {
            return That.Bind<TService> (concrete);
        }

        public static IBindData Bind<TService> (Func<object> concrete) {
            return That.Bind<TService> (concrete);
        }

        public static IBindData Bind (string service, Func<IContainer, object[], object> concrete) {
            return That.Bind (service, concrete);
        }

        public static IBindData Singleton<TService, TConcrete> () {
            return That.Singleton<TService, TConcrete> ();
        }

        public static IBindData Singleton<TService> () {
            return That.Singleton<TService> ();
        }

        public static IBindData Singleton<TService> (Func<IContainer, object[], object> concrete) {
            return That.Singleton<TService> (concrete);
        }

        public static IBindData Singleton<TService> (Func<object[], object> concrete) {
            return That.Singleton<TService> (concrete);
        }

        public static IBindData Singleton<TService> (Func<object> concrete) {
            return That.Singleton<TService> (concrete);
        }

        public static IBindData Singleton (string service, Func<IContainer, object[], object> concrete) {
            return That.Singleton (service, concrete);
        }

        public static void Unbind (string service) {
            That.Unbind (service);
        }

        public static void Unbind<TService> () {
            That.Unbind<TService> ();
        }

        public static object[] Tagged (string tag) {
            return That.Tagged (tag);
        }

        public static void Tag (string tag, params string[] service) {
            That.Tag (tag, service);
        }

        public static void Tag<TService> (string tag) {
            That.Tag<TService> (tag);
        }

        public static object Instance (string service, object instance) {
            return That.Instance (service, instance);
        }

        public static void Instance<TService> (object instance) {
            That.Instance<TService> (instance);
        }

        public static bool Release (string service) {
            return That.Release (service);
        }

        public static bool Release<TService> () {
            return That.Release<TService> ();
        }

        public static bool Release (ref object[] instances, bool reverse = true) {
            return That.Release (ref instances, reverse);
        }

        public static object Make (string service, params object[] userParams) {
            return That.Make (service, userParams);
        }

        public static TService Make<TService> (params object[] userParams) {
            return That.Make<TService> (userParams);
        }

        public static object Make (Type type, params object[] userParams) {
            return That.Make (type, userParams);
        }

        public static Func<object> Factory (string service, params object[] userParams) {
            return That.Factory (service, userParams);
        }

        public static Func<TService> Factory<TService> (params object[] userParams) {
            return That.Factory<TService> (userParams);
        }

        public static IContainer OnRelease (Action<IBindData, object> action) {
            return That.OnRelease (action);
        }

        public static IContainer OnRelease (Action<object> callback) {
            return That.OnRelease (callback);
        }

        public static IContainer OnRelease<TWhere> (Action<TWhere> closure) {
            return That.OnRelease (closure);
        }

        public static IContainer OnRelease<TWhere> (Action<IBindData, TWhere> closure) {
            return That.OnRelease (closure);
        }

        public static IContainer OnResolving (Action<IBindData, object> closure) {
            return That.OnResolving (closure);
        }

        public static IContainer OnResolving (Action<object> callback) {
            return That.OnResolving (callback);
        }

        public static IContainer OnResolving<TWhere> (Action<TWhere> closure) {
            return That.OnResolving (closure);
        }

        public static IContainer OnResolving<TWhere> (Action<IBindData, TWhere> closure) {
            return That.OnResolving (closure);
        }

        public static IContainer OnAfterResolving (Action<IBindData, object> closure) {
            return That.OnAfterResolving (closure);
        }

        public static IContainer OnAfterResolving (Action<object> closure) {
            return That.OnAfterResolving (closure);
        }

        public static IContainer OnAfterResolving<TWhere> (Action<TWhere> closure) {
            return That.OnAfterResolving (closure);
        }

        public static IContainer OnAfterResolving<TWhere> (Action<IBindData, TWhere> closure) {
            return That.OnAfterResolving (closure);
        }

        public static void Watch<TService> (Action method) {
            That.Watch<TService> (method);
        }

        public static void Watch<TService> (Action<TService> method) {
            That.Watch (method);
        }

        public static string Type2Service (Type type) {
            return That.Type2Service (type);
        }

        public static string Type2Service<TService> () {
            return That.Type2Service<TService> ();
        }
    }
}