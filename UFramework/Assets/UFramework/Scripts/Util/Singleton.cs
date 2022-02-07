using System;
using System.Reflection;
namespace UFramework.Util {

    public class Singleton<T> where T : Singleton<T> {
        private static T _instance;

        public static T Instance {
            get {
                if (_instance == null) {
                    Type type = typeof (T);
                    ConstructorInfo[] constructors = type.GetConstructors (BindingFlags.Instance | BindingFlags.NonPublic);
                    ConstructorInfo constructor = Array.Find (constructors, (c) => c.GetParameters ().Length == 0);
                    if (constructor == null) {
                        throw new System.Exception ($"{type} cannot find non param constructor.");
                    }

                    _instance = constructor.Invoke (null) as T;
                }
                return _instance;
            }
        }

    }

}