using UFramework.Container;
using UFramework.EventDispatcher;

namespace UFramework.Core
{
    public interface IApplication : IContainer
    {
        bool IsMainThread { get; }

        DebugLevel DebugLevel { get; set; }

        IEventDispatcher GetDispatcher();

        void Register(IServiceProvider provider, bool force = false);

        bool IsRegistered(IServiceProvider provider);

        long GetRuntimeId();

        void Terminate();
    }
}