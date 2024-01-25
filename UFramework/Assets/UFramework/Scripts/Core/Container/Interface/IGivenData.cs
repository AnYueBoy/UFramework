using System;

namespace UFramework
{
    public interface IGivenData<TReturn> where TReturn : IBindable
    {
        /// <summary>
        /// 给定的服务
        /// </summary>
        /// <returns>IBindData 实例</returns>
        TReturn Given(string service);

        TReturn Gieven<TService>();

        TReturn Given(Func<object> closure);
    }
}