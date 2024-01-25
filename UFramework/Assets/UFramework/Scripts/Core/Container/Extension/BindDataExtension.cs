using System;

namespace UFramework
{
    /// <summary>
    /// BindData扩展类
    /// </summary>
    public static class BindDataExtension
    {
        public static IBindData Alias<TAlias>(this IBindData bindData)
        {
            return bindData.Alias(bindData.Container.Type2Service(typeof(TAlias)));
        }

        public static IBindData OnResolving(this IBindData bindData, Action closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.OnResolving((_, instance) => { closure(); });
        }

        public static IBindData OnResolving(this IBindData bindData, Action<object> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.OnResolving((_, instance) => { closure(instance); });
        }

        public static IBindData OnResolving<T>(this IBindData bindData, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.OnResolving((_, instance) =>
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
            return bindData.OnResolving((bind, instance) =>
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
            return bindData.OnAfterResolving((_, instance) => { closure(); });
        }

        public static IBindData OnAfterResolving(this IBindData bindData, Action<object> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.OnAfterResolving((_, instance) => { closure(instance); });
        }

        public static IBindData OnAfterResolving<T>(this IBindData bindData, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.OnAfterResolving((_, instance) =>
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
            return bindData.OnAfterResolving((bind, instance) =>
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
            return bindData.OnRelease((_, __) => { closure(); });
        }

        public static IBindData OnRelease(this IBindData bindData, Action<object> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.OnRelease((_, instance) => { closure(instance); });
        }

        public static IBindData OnRelease<T>(this IBindData bindData, Action<T> closure)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            return bindData.OnRelease((_, instance) =>
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
            return bindData.OnRelease((bind, instance) =>
            {
                if (instance is T)
                {
                    closure(bind, (T)instance);
                }
            });
        }
    }
}