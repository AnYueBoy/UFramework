using System;

namespace UFramework.Core.Container
{
    public interface IContainer
    {
        /// <summary>
        /// 将类型转为服务名
        /// </summary>
        string Type2Service(Type type);
    }
}