using System.Runtime.CompilerServices;

namespace UFramework.Coroutine
{
    public interface ICriticalAwaiter : IAwaiter, ICriticalNotifyCompletion
    {
    }

    public interface ICriticalAwaiter<out TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion
    {
    }
}