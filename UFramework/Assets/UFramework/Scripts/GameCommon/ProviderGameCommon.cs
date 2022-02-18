using UFramework.Core;
namespace UFramework.GameCommon {

    public class ProviderGameCommon : IServiceProvider {
        public void Init () { }

        public void Register () {
            App.Singleton<IUIManager, UIManager> ();
            App.Singleton<IAssetsManager, AssetsManager> ();
            App.Singleton<IAudioManager, AudioManager> ();
            App.Singleton<IObjectPool, ObjectPool> ();
        }
    }
}