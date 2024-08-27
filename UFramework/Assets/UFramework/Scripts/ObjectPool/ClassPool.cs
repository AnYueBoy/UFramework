using System;
using System.Collections.Generic;

namespace UFramework
{
    /// <summary>
    /// 此类允许从对象池中生成类
    /// </summary>
    public static class ClassPool<T> where T : class
    {
        private static List<T> cached = new List<T>();

        /// <summary>
        /// 如果对象池中有对象化则返回空闲对象否则返回Null
        /// </summary>
        public static T Spawn()
        {
            var count = cached.Count;
            if (count <= 0)
            {
                return null;
            }

            var index = count - 1;
            var instance = cached[index];
            cached.RemoveAt(index);
            return instance;
        }

        public static T Spawn(Action<T> onSpawn)
        {
            var instance = default(T);
            TrySpawn(onSpawn, ref instance);
            return instance;
        }

        public static T Spawn(Predicate<T> match)
        {
            var instance = default(T);
            TrySpawn(match, ref instance);
            return instance;
        }

        public static T Spawn(Predicate<T> match, Action<T> onSpawn)
        {
            var instance = default(T);
            TrySpawn(match, onSpawn, ref instance);
            return instance;
        }

        public static bool TrySpawn(Action<T> onSpawn, ref T instance)
        {
            var count = cached.Count;
            if (count <= 0)
            {
                return false;
            }

            var index = count - 1;
            instance = cached[index];
            cached.RemoveAt(index);

            onSpawn?.Invoke(instance);
            return true;
        }

        public static bool TrySpawn(Predicate<T> match, ref T instance)
        {
            var index = cached.FindIndex(match);
            if (index < 0)
            {
                return false;
            }

            instance = cached[index];
            cached.RemoveAt(index);
            return true;
        }

        public static bool TrySpawn(Predicate<T> match, Action<T> onSpawn, ref T instance)
        {
            var index = cached.FindIndex(match);
            if (index < 0)
            {
                return false;
            }

            instance = cached[index];
            cached.RemoveAt(index);
            onSpawn(instance);
            return true;
        }

        public static void Despawn(T instance)
        {
            if (instance == null)
            {
                return;
            }

            cached.Add(instance);
        }

        public static void Despawn(T instance, Action<T> onDespawn)
        {
            if (instance == null)
            {
                return;
            }

            onDespawn(instance);
            cached.Add(instance);
        }
    }
}