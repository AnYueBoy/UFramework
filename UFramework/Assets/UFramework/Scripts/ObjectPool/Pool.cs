using System.Collections.Generic;
using UnityEngine;

namespace UFramework
{
    public static class Pool
    {
        /// <summary>
        /// 从此生成预制件时，预制件与池子的关系将存储在此，以便可以快速回收
        /// 动态创建的池子
        /// </summary>
        public static Dictionary<GameObject, ObjectPool> Links = new Dictionary<GameObject, ObjectPool>();

        public static T Spawn<T>(T prefab, Transform parent, bool worldPositionStay = false) where T : Component
        {
            if (prefab == null)
            {
                Debug.LogError($"预制为空");
                return null;
            }

            var clone = Spawn(prefab.gameObject, parent, worldPositionStay);
            return clone != null ? clone.GetComponent<T>() : null;
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Component
        {
            if (prefab == null)
            {
                Debug.LogError("预制为空");
                return null;
            }

            var clone = Spawn(prefab.gameObject, position, rotation, parent);
            return clone != null ? clone.GetComponent<T>() : null;
        }

        public static T Spawn<T>(T prefab)
            where T : Component
        {
            if (prefab == null)
            {
                Debug.LogError("预制为空");
                return null;
            }

            var clone = Spawn(prefab.gameObject);
            return clone != null ? clone.GetComponent<T>() : null;
        }

        public static GameObject Spawn(GameObject prefab, Transform parent, bool worldPositionStay = false)
        {
            if (prefab == null)
            {
                Debug.LogError($"预制为空");
                return null;
            }

            var trans = prefab.transform;

            if (parent != null && worldPositionStay)
            {
                return Spawn(prefab, prefab.transform.position, Quaternion.identity, Vector3.one, parent,
                    worldPositionStay);
            }

            return Spawn(prefab, trans.localPosition, trans.localRotation, trans.localScale, parent, false);
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation,
            Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError($"预制为空");
                return null;
            }

            if (parent != null)
            {
                position = parent.InverseTransformPoint(position);
                rotation = Quaternion.Inverse(parent.rotation) * rotation;
            }

            return Spawn(prefab, position, rotation, prefab.transform.localScale, parent, false);
        }

        public static GameObject Spawn(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError($"预制为空");
                return null;
            }

            var transform = prefab.transform;
            return Spawn(prefab, transform.localPosition, transform.localRotation, transform.localScale, null, false);
        }

        private static GameObject Spawn(GameObject prefab, Vector3 localPosition, Quaternion localRotation,
            Vector3 localScale, Transform parent, bool worldPositionStays)
        {
            if (prefab == null)
            {
                Debug.LogError("生成的对象预制为空");
                return null;
            }

            var pool = default(ObjectPool);

            // 查找此预制关联的对象池
            if (!ObjectPool.TryFindPoolByPrefab(prefab, ref pool))
            {
                // 查找不到，创建一个池子
                pool = new GameObject("Pool (" + prefab.name + ")").AddComponent<ObjectPool>();
                pool.Prefab = prefab;
            }

            var clone = default(GameObject);
            if (pool.TrySpawn(ref clone, localPosition, localRotation, localScale, parent, worldPositionStays))
            {
                Links.Add(clone, pool);
                return clone;
            }

            return null;
        }

        /// <summary>
        /// 调用所有启动的对象池的回收方法
        /// </summary>
        public static void DeSpawnAll()
        {
            foreach (var instance in ObjectPool.Instances)
            {
                instance.DeSpawnAll(false);
            }

            Links.Clear();
        }

        /// <summary>
        /// 可通过组件并延迟回收
        /// </summary>
        public static void Despawn(Component clone, float delay = 0.0f)
        {
            if (clone != null) Despawn(clone.gameObject, delay);
        }

        public static void Despawn(GameObject clone, float delay = 0.0f)
        {
            if (clone == null)
            {
                Debug.LogWarning("回收的对象为空.", clone);
                return;
            }

            if (Links.TryGetValue(clone, out var pool))
            {
                Links.Remove(clone);
                pool.Despawn(clone, delay);
                return;
            }

            if (ObjectPool.TryFindPoolByClone(clone, ref pool))
            {
                pool.Despawn(clone, delay);
                return;
            }

            Debug.LogWarning(
                "要回收的对象不是该池子产生的。或者池子已经销毁",
                clone);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(clone);
                return;
            }
#endif

            if (delay > 0)
            {
                Object.Destroy(clone, delay);
            }
            else
            {
                Object.Destroy(clone);
            }
        }

        /// <summary>
        /// 通过组件分离对应的克隆对象，不再受池子管理
        /// </summary>
        public static void Detach(Component clone, bool detachFromPool = true)
        {
            if (clone != null)
            {
                Detach(clone.gameObject, detachFromPool);
            }
        }

        public static void Detach(GameObject clone, bool detachFromPool)
        {
            if (clone == null)
            {
                Debug.LogWarning("要分离的对象为空.", clone);
                return;
            }

            // 分离对象从动态关系字典中移除
            if (!detachFromPool)
            {
                Links.Remove(clone);
                return;
            }

            if (Links.TryGetValue(clone, out var pool))
            {
                Links.Remove(clone);
                pool.Detach(clone, false);
                return;
            }

            if (ObjectPool.TryFindPoolByClone(clone, ref pool))
            {
                pool.Detach(clone, false);
                return;
            }

            Debug.LogWarning(
                "要分离的对象 不属于任何池子或者对应的池子已经销毁",
                clone);
        }
    }
}