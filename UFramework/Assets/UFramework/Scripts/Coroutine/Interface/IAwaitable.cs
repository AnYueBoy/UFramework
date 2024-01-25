namespace UFramework
{
    /// <summary>
    /// 表示一个可等待对象，如果一个方法返回此类型的实例，则此方法可以使用 `await` 异步等待。
    /// </summary>
    public interface IAwaitable<out TAwaiter> where TAwaiter : IAwaiter
    {
        TAwaiter GetAwaiter();
    }

    /// <summary>
    /// 表示一个包含返回值的可等待对象，如果一个方法返回此类型的实例，则此方法可以使用 `await` 异步等待返回值。
    /// </summary>
    public interface IAwaitable<out TAwaiter, out TResult> where TAwaiter : IAwaiter<TResult>
    {
        TAwaiter GetAwaiter();
    }
}