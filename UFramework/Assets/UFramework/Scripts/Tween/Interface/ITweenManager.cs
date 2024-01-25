namespace UFramework {
    public interface ITweenManager {

        void LocalUpdate (float dt);

        T2 SpawnTweener<T1, T2> () where T2 : Tweener<T1>, new ();

        void RemoveTweener(ITweener tweener);
    }
}