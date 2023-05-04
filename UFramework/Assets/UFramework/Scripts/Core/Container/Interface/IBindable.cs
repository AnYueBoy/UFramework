namespace UFramework.Core.Container
{
    /// <summary>
    /// 所有可绑定数据类实现的接口。
    /// </summary>
    public interface IBindable
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        string Service { get; }

        /// <summary>
        /// 服务容器
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// 从容器中解绑服务，如果服务是单例则会被自动释放
        /// </summary>
        void Unbind();
    }

    public interface IBindable<TReturn> : IBindable where TReturn : IBindable
    {
        /// <summary>
        /// 获取服务指定的需求上下文数据
        /// </summary>
        /// <returns>上下文中给定的关系</returns>
        IGivenData<TReturn> Needs(string service);

        IGivenData<TReturn> Needs<TService>();
    }
}