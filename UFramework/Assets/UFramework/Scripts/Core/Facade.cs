using UFramework.Container;

namespace UFramework.Core {
    public abstract class Facade<TService> {
        private static readonly string Service;
        private static TService that;
        private static IBindData binder;
        private static bool inited;
        private static bool released;

        /// <summary>
        /// Initializes static members of the Facade class.
        /// </summary>
        static Facade () {
            Service = App.Type2Service (typeof (TService));
            App.OnNewApplication += app => {
                that = default;
                binder = null;
                inited = false;
                released = false;
            };
        }

        public static TService That => HasInstance ? that : Resolve ();

        /// <summary>
        /// Gets a value indicating whether the resolved instance is exists in the facade.
        /// <para>If it is a non-static binding then return forever false.</para>
        /// </summary>
        internal static bool HasInstance => binder != null && binder.IsStatic && !released && that != null;

        /// <summary>
        /// Resolve the object instance.
        /// </summary>
        internal static TService Make (params object[] userParams) {
            return HasInstance ? that : Resolve (userParams);
        }

        private static TService Resolve (params object[] userParams) {
            released = false;

            if (!inited && (App.IsResolved (Service) || App.CanMake (Service))) {
                App.Watch<TService> (ServiceRebound);
                inited = true;
            } else if (binder != null && !binder.IsStatic) {
                // If it has been initialized, the binder has been initialized.
                // Then judging in advance can optimize performance without
                // going through a hash lookup.
                return Build (userParams);
            }

            var newBinder = App.GetBind (Service);
            if (newBinder == null || !newBinder.IsStatic) {
                binder = newBinder;
                return Build (userParams);
            }

            Rebind (newBinder);
            return that = Build (userParams);
        }

        /// <summary>
        /// When the resolved object is released.
        /// </summary>
        private static void OnRelease (IBindData oldBinder, object instance) {
            if (oldBinder != binder) {
                return;
            }

            that = default;
            released = true;
        }

        /// <summary>
        /// When the resolved object is rebound.
        /// </summary>
        private static void ServiceRebound (TService newService) {
            var newBinder = App.GetBind (Service);
            Rebind (newBinder);
            that = (newBinder == null || !newBinder.IsStatic) ? default : newService;
        }

        /// <summary>
        /// Rebinding the bound data to given binder.
        /// </summary>
        private static void Rebind (IBindData newBinder) {
            binder = newBinder;
        }

        /// <summary>
        /// Resolve facade object from the container.
        /// </summary>
        /// <returns>The resolved object.</returns>
        private static TService Build (params object[] userParams) {
            return (TService) App.Make (Service, userParams);
        }
    }
}