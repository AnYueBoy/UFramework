using System;

namespace UFramework
{
    /// <summary>
    /// BindData扩展类
    /// </summary>
    public static class BindDataExtension
    {
        public static IBindData OnResolving(this IBindData bindData, Action closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterResolvingHandler((_, instance) => { closure(); });
        }

        public static IBindData OnResolving(this IBindData bindData, Action<object> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterResolvingHandler((_, instance) => { closure(instance); });
        }

        public static IBindData OnResolving<T>(this IBindData bindData, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterResolvingHandler((_, instance) =>
            {
                if (instance is T)
                {
                    closure((T)instance);
                }
            });
        }

        public static IBindData OnResolving<T>(this IBindData bindData, Action<IBindData, T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterResolvingHandler((bind, instance) =>
            {
                if (instance is T)
                {
                    closure(bind, (T)instance);
                }
            });
        }

        public static IBindData OnAfterResolving(this IBindData bindData, Action closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterAfterResolvingHandler((_, instance) => { closure(); });
        }

        public static IBindData OnAfterResolving(this IBindData bindData, Action<object> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterAfterResolvingHandler((_, instance) => { closure(instance); });
        }

        public static IBindData OnAfterResolving<T>(this IBindData bindData, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterAfterResolvingHandler((_, instance) =>
            {
                if (instance is T)
                {
                    closure((T)instance);
                }
            });
        }

        public static IBindData OnAfterResolving<T>(this IBindData bindData, Action<IBindData, T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterAfterResolvingHandler((bind, instance) =>
            {
                if (instance is T)
                {
                    closure(bind, (T)instance);
                }
            });
        }

        public static IBindData OnRelease(this IBindData bindData, Action closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterReleaseHandler((_, __) => { closure(); });
        }

        public static IBindData OnRelease(this IBindData bindData, Action<object> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterReleaseHandler((_, instance) => { closure(instance); });
        }

        public static IBindData OnRelease<T>(this IBindData bindData, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterReleaseHandler((_, instance) =>
            {
                if (instance is T)
                {
                    closure((T)instance);
                }
            });
        }

        public static IBindData OnRelease<T>(this IBindData bindData, Action<IBindData, T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.RegisterReleaseHandler((bind, instance) =>
            {
                if (instance is T)
                {
                    closure(bind, (T)instance);
                }
            });
        }
    }
}