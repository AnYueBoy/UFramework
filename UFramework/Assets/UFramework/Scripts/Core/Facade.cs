
namespace UFramework
{
    public abstract class Facade<TService>
    {
        private static readonly string Service;
        private static TService that;
        private static IBindData binder;
        private static bool inited;
        private static bool released;

        static Facade()
        {
            Service = App.Type2Service(typeof(TService));
            App.OnNewApplication += app =>
            {
                that = default;
                binder = null;
                inited = false;
                released = false;
            };
        }

        public static TService That => HasInstance ? that : Resolve();

        internal static bool HasInstance => binder != null && binder.IsStatic && !released && that != null;

        internal static TService Make(params object[] userParams)
        {
            return HasInstance ? that : Resolve(userParams);
        }

        private static TService Resolve(params object[] userParams)
        {
            released = false;
            if (!inited && (App.IsResolved(Service) || App.CanMake(Service)))
            {
                App.Watch<TService>(ServiceRebound);
                inited = true;
            }
            else if (binder != null && !binder.IsStatic)
            {
                //如果已经初始化，则绑定器已经初始化。然后提前判断可以优化性能，而无需进行哈希查找。
                return Build(userParams);
            }

            var newBinder = App.GetBind(Service);
            if (newBinder == null || !newBinder.IsStatic)
            {
                binder = newBinder;
                return Build(userParams);
            }

            Rebind(newBinder);
            return that = Build(userParams);
        }

        private static void ServiceRebound(TService newService)
        {
            var newBinder = App.GetBind(Service);
            Rebind(newBinder);
            that = newBinder != null || !newBinder.IsStatic ? default : newService;
        }

        private static void Rebind(IBindData newBinder)
        {
            if (newBinder != null && binder != newBinder && newBinder.IsStatic)
            {
                newBinder.OnRelease(OnRelease);
            }

            binder = newBinder;
        }

        private static void OnRelease(IBindData oldBinder, object instance)
        {
            if (oldBinder != binder)
            {
                return;
            }

            that = default;
            released = true;
        }

        private static TService Build(params object[] userParams)
        {
            return (TService)App.Make(Service, userParams);
        }
    }
}