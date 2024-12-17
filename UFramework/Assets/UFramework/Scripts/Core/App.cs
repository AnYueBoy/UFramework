using System;
using System.Reflection;

namespace UFramework
{
    public abstract class App
    {
        private static IApplication that;

        public static event Action<IApplication> OnNewApplication
        {
            add
            {
                RaiseOnNewApplication += value;
                if (That != null)
                {
                    value?.Invoke(That);
                }
            }
            remove => RaiseOnNewApplication -= value;
        }

        private static event Action<IApplication> RaiseOnNewApplication;

        public static IApplication That
        {
            get { return that; }

            set
            {
                that = value;
                RaiseOnNewApplication?.Invoke(that);
            }
        }

        public static bool IsMainThread => That.IsMainThread;

        public static DebugLevel DebugLevel
        {
            get => That.DebugLevel;
            set => That.DebugLevel = value;
        }

        public static void Terminate()
        {
            That.Terminate();
        }

        public static void Register(IServiceProvider provider, bool force = false)
        {
            That.Register(provider, force);
        }

        public static bool IsRegistered(IServiceProvider provider)
        {
            return That.IsRegistered(provider);
        }

        public static long GetRuntimeId()
        {
            return That.GetRuntimeId();
        }

        public static void UnbindMethod(object target)
        {
            That.UnbindMethod(target);
        }

        public static object Invoke(string method, params object[] userParams)
        {
            return That.Invoke(method, userParams);
        }

        public static IBindData Bind<TService>()
        {
            return That.Bind<TService>();
        }

        public static IBindData Bind<TService, TConcrete>()
        {
            return That.Bind<TService, TConcrete>();
        }

        public static IBindData Bind(string service, Type concrete, bool isStatic)
        {
            return That.Bind(service, concrete, isStatic);
        }

        public static IBindData Bind(string service, Func<IContainer, object[], object> concrete, bool isStatic)
        {
            return That.Bind(service, concrete, isStatic);
        }

        public static IBindData Bind<TService>(Func<IContainer, object[], object> concrete)
        {
            return That.Bind<TService>(concrete);
        }

        public static IBindData Bind<TService>(Func<object[], object> concrete)
        {
            return That.Bind<TService>(concrete);
        }

        public static IBindData Bind<TService>(Func<object> concrete)
        {
            return That.Bind<TService>(concrete);
        }

        public static IBindData Bind(string service, Func<IContainer, object[], object> concrete)
        {
            return That.Bind(service, concrete);
        }

        public static IBindData Singleton<TService, TConcrete>()
        {
            return That.Singleton<TService, TConcrete>();
        }

        public static IBindData Singleton<TService>()
        {
            return That.Singleton<TService>();
        }

        public static IBindData Singleton<TService>(Func<IContainer, object[], object> concrete)
        {
            return That.Singleton<TService>(concrete);
        }

        public static IBindData Singleton<TService>(Func<object[], object> concrete)
        {
            return That.Singleton<TService>(concrete);
        }

        public static IBindData Singleton<TService>(Func<object> concrete)
        {
            return That.Singleton<TService>(concrete);
        }

        public static IBindData Singleton(string service, Func<IContainer, object[], object> concrete)
        {
            return That.Singleton(service, concrete);
        }

        public static IMethodBind BindMethod(string method, object target, MethodInfo call)
        {
            return That.BindMethod(method, target, (MethodInfo)call);
        }

        public static IMethodBind BindMethod(string method, object target,
            string call = null)
        {
            return That.BindMethod(method, target, call);
        }

        public static IMethodBind BindMethod(string method, Func<object> callback)
        {
            return That.BindMethod(method, callback);
        }

        public static IMethodBind BindMethod<T1>(string method, Func<T1, object> callback)
        {
            return That.BindMethod(method, callback);
        }

        public static IMethodBind BindMethod<T1, T2>(string method, Func<T1, T2, object> callback)
        {
            return That.BindMethod(method, callback);
        }

        public static IMethodBind BindMethod<T1, T2, T3>(string method, Func<T1, T2, T3, object> callback)
        {
            return That.BindMethod(method, callback);
        }

        public static IMethodBind BindMethod<T1, T2, T3, T4>(string method, Func<T1, T2, T3, T4, object> callback)
        {
            return That.BindMethod(method, callback);
        }

        public static void Unbind(string service)
        {
            That.Unbind(service);
        }

        public static void Unbind<TService>()
        {
            That.Unbind<TService>();
        }

        public static object[] Tagged(string tag)
        {
            return That.Tagged(tag);
        }

        public static void Tag(string tag, params string[] service)
        {
            That.Tag(tag, service);
        }

        public static void Tag<TService>(string tag)
        {
            That.Tag<TService>(tag);
        }

        public static object Instance(string service, object instance)
        {
            return That.Instance(service, instance);
        }

        public static void Instance<TService>(object instance)
        {
            That.Instance<TService>(instance);
        }

        public static bool Release(string service)
        {
            return That.Release(service);
        }

        public static bool Release<TService>()
        {
            return That.Release<TService>();
        }

        public static bool Release(ref object[] instances, bool reverse = true)
        {
            return That.Release(ref instances, reverse);
        }

        public static object Call(object instance, MethodInfo methodInfo, params object[] userParams)
        {
            return That.Call(instance, methodInfo, userParams);
        }

        public static object Call(object instance, string method, params object[] userParams)
        {
            return That.Call(instance, method, userParams);
        }

        public static void Call<T1>(Action<T1> method, params object[] userParams)
        {
            That.Call(method, userParams);
        }

        public static void Call<T1, T2>(Action<T1, T2> method, params object[] userParams)
        {
            That.Call(method, userParams);
        }

        public static void Call<T1, T2, T3>(Action<T1, T2, T3> method, params object[] userParams)
        {
            That.Call(method, userParams);
        }

        public static void Call<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, params object[] userParams)
        {
            That.Call(method, userParams);
        }

        public static Action Wrap<T1>(Action<T1> method, params object[] userParams)
        {
            return That.Wrap(method, userParams);
        }

        public static Action Wrap<T1, T2>(Action<T1, T2> method, params object[] userParams)
        {
            return That.Wrap(method, userParams);
        }

        public static Action Wrap<T1, T2, T3>(Action<T1, T2, T3> method, params object[] userParams)
        {
            return That.Wrap(method, userParams);
        }

        public static Action Wrap<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, params object[] userParams)
        {
            return That.Wrap(method, userParams);
        }

        public static object Make(string service, params object[] userParams)
        {
            return That.Make(service, userParams);
        }

        public static TService Make<TService>(params object[] userParams)
        {
#if UFRAMEWORK_PERFORMCE
            return Facade<TService>.Make(userParams);
#else
            return That.Make<TService>(userParams);
#endif
        }

        public static string Type2Service(Type type)
        {
            return That.Type2Service(type);
        }

        public static string Type2Service<TService>()
        {
            return That.Type2Service<TService>();
        }
    }
}