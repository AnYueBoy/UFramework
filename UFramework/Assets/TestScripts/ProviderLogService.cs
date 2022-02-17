using UFramework.Core;
public class ProviderLogService : IServiceProvider {
    public void Init () { }

    public void Register () {
        App.Singleton<ILogManager, LogManager> ();
    }
}