using System;
using UFramework.Util;

namespace UFramework.Core.Container
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
    }
}