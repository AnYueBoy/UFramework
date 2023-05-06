using System;

namespace UFramework.Core.Container
{
    /// <summary>
    /// 表示类的构造函数允许传入一个原始类型（包括字符串），以转换为当前的类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class VariantAttribute : Attribute
    {
    }
}