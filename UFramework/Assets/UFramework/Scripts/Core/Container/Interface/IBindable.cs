namespace UFramework
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
        /// 获取服务所属的容器
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// 从容器中解绑服务，
        /// 如果被绑定的类是单例化的且已经被构建，
        /// 那么解除绑定时会自动释放已经被生成的单例，同时触发服务构建事件
        /// </summary>
        void Unbind();
    }

    public interface IBindable<TReturn> : IBindable
        where TReturn : IBindable
    {
        /// <summary>
        /// 获取服务指定的需求上下文数据
        /// </summary>
        /// <returns>上下文中给定的关系</returns>
        /// 用例：通过绑定类型名来描述上下文
        /// App.Singleton<ScreenshotUpload>().Needs<IDisk>().Given(()=> FileSystem.Disk("aliyun-oss"))
        //  通过绑定变量名来描述上下文 绑定的变量名必须以$开头，对于变量名的大小写敏感。
        // App.Singleton<ScreenshotUpload>().Needs("$disk").Given(()=> FileSystem.Disk("aliyun-oss"))
        // 当构建截图上传服务时，框架查找截图上传服务构造函数中的disk变量，并为其给定指定实现。
        IGivenData<TReturn> Needs(string service);

        IGivenData<TReturn> Needs<TService>();
    }
}