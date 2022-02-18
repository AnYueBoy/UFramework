using UFramework.Core;
namespace UFramework.Tween {

    public class ProviderTweener : IServiceProvider {
        public void Init () { }

        public void Register () {
            App.Singleton<ITweenManager, TweenManager> ();
        }
    }
}