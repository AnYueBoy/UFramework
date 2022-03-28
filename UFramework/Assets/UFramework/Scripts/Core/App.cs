using System;
using UFramework.Container;

namespace UFramework.Core {
    public abstract class App {
        private static IApplication that;

        private static event Action<IApplication> RaiseOnNewApplication;

        public static event Action<IApplication> OnNewApplication {
            add {
                RaiseOnNewApplication += value;
                if (That != null) {
                    value?.Invoke (That);
                }
            }
            remove => RaiseOnNewApplication -= value;
        }

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

        public static IBindData Bind (string service, Type concrete, bool isStatic) {
            return That.Bind (service, concrete, isStatic);
        }

        public static IBindData Bind (string service, Func<IContainer, object[], object> concrete, bool isStatic) {
            return That.Bind (service, concrete, isStatic);
        }

        public static IBindData Singleton<TService, TConcrete> () {
            return That.Singleton<TService, TConcrete> ();
        }

        public static IBindData Singleton<TService> () {
            return That.Singleton<TService> ();
        }

        public static void Unbind (string service) {
            That.Unbind (service);
        }

        public static void Unbind<TService> () {
            That.Unbind<TService> ();
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

        public static string Type2Service (Type type) {
            return That.Type2Service (type);
        }

        public static string Type2Service<TService> () {
            return That.Type2Service<TService> ();
        }

        #region 事件周期

        public static IContainer OnRelease (Action<IBindData, object> action) {
            return That.OnRelease (action);
        }

        public static IContainer OnAfterResolving (Action<IBindData, object> closure) {
            return That.OnAfterResolving (closure);
        }
        #endregion
    }
}