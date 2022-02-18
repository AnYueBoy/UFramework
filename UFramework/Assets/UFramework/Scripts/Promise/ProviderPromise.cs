using UFramework.Core;
namespace UFramework.Promise {
    public class ProviderPromise : IServiceProvider {
        public void Init () { }

        public void Register () {
            App.Singleton<IPromiseTimer, PromiseTimer> ();
        }
    }
}