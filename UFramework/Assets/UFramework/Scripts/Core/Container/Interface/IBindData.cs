using System;

namespace UFramework
{
    /// <summary>
    /// 绑定数据 表示与指定服务相关的关系数据。
    /// </summary>
    public interface IBindData : IBindable
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
    }
}