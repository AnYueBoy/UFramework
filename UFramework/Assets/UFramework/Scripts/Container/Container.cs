using System;

namespace UFramework.Container {
    public class Container : IContainer {
        public string Type2Service (Type type) {
            return type.ToString ();
        }
    }
}