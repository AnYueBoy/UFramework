namespace UFramework
{
    public interface ICoroutine : IAwaitable<CoroutineAwaiter>
    {
        CoroutineState State { get; }

        void Complete();

        void Pause();

        void Resume();
    }
}