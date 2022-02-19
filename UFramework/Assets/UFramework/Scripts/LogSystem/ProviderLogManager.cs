using UFramework.Core;
namespace UFramework.LogSystem {

    public class ProviderLogManager : IServiceProvider {
        public void Init () {
            App.Make<ILogManager> ().Init ();
        }

        public void Register () {
            App.Singleton<ILogManager, LogManager> ();
        }
    }
}