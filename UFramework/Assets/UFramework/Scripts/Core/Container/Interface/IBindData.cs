using System;

namespace UFramework
{
    /// <summary>
    /// 绑定数据 表示与指定服务相关的关系数据。
    /// </summary>
    public interface IBindData : IBindable<IBindData>
    {
        /// <summary>
        /// 获取返回具体服务的委托
        /// </summary>
        Func<IContainer, object[], object> Concrete { get; }

        /// <summary>
        /// 表示该服务是否是单例（static）
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// 服务Tag(用于批量释放一组Tag的服务)
        /// </summary>
        IBindData Tag(string tag);

        /// <summary>
        /// 注册Resolving回调
        /// </summary>
        IBindData OnResolving(Action<IBindData, object> closure);

        /// <summary>
        /// 注册Resolving之后的回调
        /// </summary>
        IBindData OnAfterResolving(Action<IBindData, object> closure);

        /// <summary>
        /// 注册Release回调
        /// </summary>
        IBindData OnRelease(Action<IBindData, object> closure);
    }
}