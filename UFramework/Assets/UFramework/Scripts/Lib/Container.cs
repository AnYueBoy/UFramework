using System;
using System.Collections.Generic;
namespace UFramework.Lib {
    public class Container {
        private Dictionary<Type, object> _instanceContainer;

        public void Register<T> (T instance) where T : class {
            Type type = typeof (T);
            if (_instanceContainer.ContainsKey (type)) {
                throw new System.Exception ($"{type} already register.");
            }

            _instanceContainer.Add (type, instance);
        }

        public T GetInstance<T> () where T : class {
            Type type = typeof (T);
            if (_instanceContainer.TryGetValue (type, out object instance)) {
                return instance as T;
            }

            return null;
        }
    }
}