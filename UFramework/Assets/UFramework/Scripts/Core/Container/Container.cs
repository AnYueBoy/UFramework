using System;

namespace UFramework.Core.Container
{
    public class Container : IContainer
    {
        public string Type2Service(Type type)
        {
            return type.ToString();
        }
    }
}