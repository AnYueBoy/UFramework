namespace UFramework.Tween {
    public interface ITweenManager {

        void localUpdate (float dt);

        T2 spawnTweener<T1, T2> () where T2 : Tweener<T1>, new ();
    }
}