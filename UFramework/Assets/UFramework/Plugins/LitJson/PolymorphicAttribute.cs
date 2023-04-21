namespace LitJson
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct,
        AllowMultiple = false, Inherited = true)]
    public class PolymorphicAttribute : Attribute
    {
    }
}