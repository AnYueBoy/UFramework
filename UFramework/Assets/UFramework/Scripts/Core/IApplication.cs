namespace UFramework
{
    public interface IApplication : IContainer
    {
        /// <summary>
        /// 是否运行在主线程
        /// </summary>
        bool IsMainThread { get; }

        /// <summary>
        /// 获取debug级别
        /// </summary>
        DebugLevel DebugLevel { get; set; }

        /// <summary>
        /// 获取事件派发器
        /// </summary>
        IEventDispatcher GetDispatcher();

        /// <summary>
        /// 通过Application注册服务提供者
        /// </summary>
        void Register(IServiceProvider provider, bool force = false);

        /// <summary>
        /// 检查指定的服务提供者是否已注册
        /// </summary>
        bool IsRegistered(IServiceProvider provider);

        /// <summary>
        /// 获取运行时的唯一id
        /// </summary>
        long GetRuntimeId();

        void Terminate();
    }
}