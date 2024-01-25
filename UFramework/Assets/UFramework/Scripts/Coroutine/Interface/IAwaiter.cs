using System.Runtime.CompilerServices;

namespace UFramework
{
    /// <summary>
    /// 用于给 await 确定异步返回的时机
    /// </summary>
    public interface IAwaiter : INotifyCompletion
    {
        /// <summary>
        /// 获取一个状态，该状态表示正在异步等待的操作已经完成（成功完成或发生了异常）；此状态会被编译器自动调用
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 此方法会被编译器在 await 结束时自动调用以获取返回状态（包括异常）。
        /// </summary>
        void GetResult();
    }

    public interface IAwaiter<out TResult> : INotifyCompletion
    {
        bool IsCompleted { get; }

        TResult GetResult();
    }
}