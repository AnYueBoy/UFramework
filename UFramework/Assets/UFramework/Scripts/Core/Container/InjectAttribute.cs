using System;

namespace UFramework.Core.Container
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置一个值，该值指示是否需要该属性。
        /// </summary>
        public bool Required { get; set; } = true;
    }
}