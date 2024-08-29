using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace UFramework
{
    [ExecuteInEditMode]
    [AddComponentMenu("Pool/Object Pool")]
    public class ObjectPool : MonoBehaviour
    {
        [Serializable]
        public class Delay
        {
            public GameObject Clone;
            public float Life;
        }

        public enum StrategyType
        {
            ActivateAndDeactivate,
            DeactivateViaHierarchy,
        }

        #region 字段

        public static List<ObjectPool> Instances = new List<ObjectPool>();

        /// <summary>
        /// 预制与对象池的关系
        /// </summary>
        private static Dictionary<GameObject, ObjectPool> prefabMap =
            new Dictionary<GameObject, ObjectPool>();

        [SerializeField] private GameObject prefab;

        public GameObject Prefab
        {
            set
            {
                if (value != prefab)
                {
                    UnregisterPrefab();
                    prefab = value;
                    RegisterPrefab();
                }
            }
            get => prefab;
        }

        [field: SerializeField] public StrategyType Strategy { get; set; }
        [field: SerializeField] public int Preload { get; set; }
        [field: SerializeField] public int Capacity { get; set; }

        // 是否不随切换场景销毁
        [field: SerializeField] public bool Persist { get; set; }

        // 生成对象是否添加索引后缀
        [field: SerializeField] public bool Stamp { get; set; }

        [field: SerializeField] public bool Warnings { get; set; } = true;


        private List<GameObject> spawnedClonesList = new List<GameObject>();
        private List<GameObject> deSpawnedClones = new List<GameObject>();

        // 所有延迟销毁的对象
        private List<Delay> delays = new List<Delay>();
        [SerializeField] private Transform deactivatedChild;

        public Transform DeactivatedChild
        {
            get
            {
                if (deactivatedChild == null)
                {
                    var child = new GameObject("DeSpawned Clones");
                    child.SetActive(false);
                    deactivatedChild = child.transform;
                    deactivatedChild.SetParent(transform, false);
                }

                return deactivatedChild;
            }
        }

        public int Spawned => spawnedClonesList.Count;

        public int DeSpawned => deSpawnedClones.Count;

        public int Total => Spawned + DeSpawned;

        #endregion

        private void RegisterPrefab()
        {
            if (prefab == null)
            {
                return;
            }

            if (prefabMap.TryGetValue(prefab, out var exitPool))
            {
                Debug.LogWarning("一个预制件绑定了多个对象池，预制件名称 (" + prefab.name + ").",
                    exitPool);
            }
            else
            {
                prefabMap.Add(prefab, this);
            }
        }

        private void UnregisterPrefab()
        {
            if (Equals(prefab, null))
            {
                return;
            }

            if (prefabMap.TryGetValue(prefab, out var existPool) && existPool == this)
            {
                prefabMap.Remove(prefab);
            }
        }

#if UNITY_EDITOR
        public bool DeSpawnedClonesMatch
        {
            get
            {
                for (int i = deSpawnedClones.Count - 1; i >= 0; i--)
                {
                    var clone = deSpawnedClones[i];
                    // 获取回收对象的源Prefab
                    if (clone != null && UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(clone) != prefab)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
#endif

        public static bool TryFindPoolByPrefab(GameObject prefab, ref ObjectPool pool)
        {
            return prefabMap.TryGetValue(prefab, out pool);
        }

        public static bool TryFindPoolByClone(GameObject clone, ref ObjectPool pool)
        {
            foreach (var instance in Instances)
            {
                // 查找列表
                for (int i = instance.spawnedClonesList.Count - 1; i >= 0; i--)
                {
                    if (instance.spawnedClonesList[i] == clone)
                    {
                        pool = instance;
                        return true;
                    }
                }
            }

            return false;
        }

        public GameObject Spawn(Transform parent, bool worldPositionStays = false)
        {
            var clone = default(GameObject);
            TrySpawn(ref clone, parent, worldPositionStays);
            return clone;
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var clone = default(GameObject);
            TrySpawn(ref clone, position, rotation, parent);
            return clone;
        }

        public bool TrySpawn(ref GameObject clone, Transform parent, bool worldPositionStays = false)
        {
            if (prefab == null && Warnings)
            {
                Debug.LogWarning($"该对象池绑定的预制为空. {this}");
                return false;
            }

            if (parent != null && worldPositionStays)
            {
                return TrySpawn(ref clone, prefab.transform.position, Quaternion.identity, Vector3.one, parent,
                    true);
            }

            return TrySpawn(ref clone, transform.localPosition, transform.localRotation, transform.localScale, parent,
                worldPositionStays);
        }

        public bool TrySpawn(ref GameObject clone, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null && Warnings)
            {
                Debug.LogWarning($"该对象池绑定的预制为空. {this}");
                return false;
            }

            if (parent != null)
            {
                position = parent.InverseTransformPoint(position);
                rotation = Quaternion.Inverse(parent.rotation) * rotation;
            }

            return TrySpawn(ref clone, position, rotation, prefab.transform.localScale, parent, false);
        }

        public bool TrySpawn(ref GameObject clone)
        {
            if (prefab == null && Warnings)
            {
                Debug.LogWarning($"该对象池绑定的预制为空. {this}");
                return false;
            }

            var trans = prefab.transform;
            return TrySpawn(ref clone, trans.localPosition, trans.localRotation, trans.localScale, null,
                false);
        }

        public bool TrySpawn(ref GameObject clone, Vector3 localPosition, Quaternion localRotation, Vector3 localScale,
            Transform parent, bool worldPositionStays)
        {
            if (prefab == null)
            {
                if (Warnings)
                {
                    Debug.LogWarning($"该对象池绑定的预制为空. {this}");
                }

                return false;
            }

            for (int i = deSpawnedClones.Count - 1; i >= 0; i--)
            {
                clone = deSpawnedClones[i];
                deSpawnedClones.RemoveAt(i);
                if (clone != null)
                {
                    SpawnClone(clone, localPosition, localRotation, localScale, parent, worldPositionStays);
                    return true;
                }

                if (Warnings)
                {
                    Debug.LogWarning($"该对象池中存在 空对象 {this}");
                }
            }

            if (Capacity <= 0 || Total < Capacity)
            {
                clone = CreateClone(localPosition, localRotation, localScale, parent, worldPositionStays);

                spawnedClonesList.Add(clone);

                if (Strategy == StrategyType.ActivateAndDeactivate)
                {
                    clone.SetActive(true);
                }

                if (clone.TryGetComponent<IPoolable>(out var com))
                {
                    com.OnSpawn();
                }

                return true;
            }

            return false;
        }

        [ContextMenu("Despawn Oldest")]
        public void DeSpawnOldest()
        {
            var clone = default(GameObject);
            TryDeSpawnOldest(ref clone, true);
        }

        private bool TryDeSpawnOldest(ref GameObject clone, bool registerDeSpawned)
        {
            while (spawnedClonesList.Count > 0)
            {
                clone = spawnedClonesList[0];
                spawnedClonesList.RemoveAt(0);

                if (clone != null)
                {
                    DespawnNow(clone, registerDeSpawned);
                    return true;
                }

                if (Warnings)
                {
                    Debug.LogWarning("对象池中包含了空的克隆对象",
                        this);
                }
            }

            return false;
        }

        [ContextMenu("Despawn All")]
        public void DespawnAll()
        {
            DeSpawnAll(true);
        }

        public void DeSpawnAll(bool cleanLinks)
        {
            for (int i = spawnedClonesList.Count - 1; i >= 0; i--)
            {
                var clone = spawnedClonesList[i];
                if (clone != null)
                {
                    if (cleanLinks)
                    {
                        // Link 处理
                        Pool.Links.Remove(clone);
                    }

                    DespawnNow(clone);
                }
            }

            spawnedClonesList.Clear();

            for (int i = delays.Count - 1; i >= 0; i--)
            {
                ClassPool<Delay>.Despawn(delays[i]);
            }

            delays.Clear();
        }

        public void Despawn(GameObject clone, float t = 0.0f)
        {
            if (clone == null)
            {
                if (Warnings)
                {
                    Debug.LogWarning("回收的克隆对象为空", this);
                }

                return;
            }

            if (t > 0)
            {
                DespawnWithDelay(clone, t);
            }
            else
            {
                TryDeSpawn(clone);
                for (var i = delays.Count - 1; i >= 0; i--)
                {
                    var delay = delays[i];

                    if (delay.Clone == clone)
                    {
                        delays.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 分离产生的对象成普通的游戏对象
        /// </summary>
        public void Detach(GameObject clone, bool cleanLinks = true)
        {
            if (clone == null)
            {
                if (Warnings)
                {
                    Debug.LogWarning($"要分离的对象不是该对象池产生的. {clone}");
                }

                return;
            }

            if (spawnedClonesList.Remove(clone) || deSpawnedClones.Remove(clone))
            {
                if (cleanLinks)
                {
                    // 关联关系清理
                    Pool.Links.Remove(clone);
                }

                // 清理已被标记为延迟回收的对象
                for (var i = delays.Count - 1; i >= 0; i--)
                {
                    var delay = delays[i];

                    if (delay.Clone == clone)
                    {
                        delays.RemoveAt(i);
                    }
                }
            }
        }

        [ContextMenu("Preload One More")]
        public void PreloadOneMore()
        {
            if (prefab == null)
            {
                if (Warnings)
                {
                    Debug.LogWarning("预加载的对象预制为空", this);
                }

                return;
            }

            var clone = CreateClone(Vector3.zero, Quaternion.identity, Vector3.one, null, false);

            deSpawnedClones.Add(clone);

            if (Strategy == StrategyType.ActivateAndDeactivate)
            {
                clone.SetActive(false);
                clone.transform.SetParent(transform, false);
            }
            else
            {
                clone.transform.SetParent(DeactivatedChild, false);
            }

            if (Warnings && Capacity > 0 && Total > Capacity)
            {
                Debug.LogWarning(
                    "预加载的对象数量超过了对象池的最大容量",
                    this);
            }
        }

        [ContextMenu("Preload All")]
        public void PreloadAll()
        {
            if (Preload <= 0)
            {
                return;
            }

            if (prefab == null)
            {
                if (Warnings)
                {
                    Debug.LogWarning("尝试预加载的池所注册的预制为空", this);
                }

                return;
            }

            for (var i = Total; i < Preload; i++)
            {
                PreloadOneMore();
            }
        }

        [ContextMenu("Clean")]
        public void Clean()
        {
            for (var i = deSpawnedClones.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(deSpawnedClones[i]);
            }

            deSpawnedClones.Clear();
        }

        public void GetClones(List<GameObject> gameObjects, bool addSpawnedClones = true,
            bool addDeSpawnedClones = true)
        {
            if (gameObjects == null)
            {
                return;
            }

            gameObjects.Clear();
            if (addSpawnedClones)
            {
                gameObjects.AddRange(spawnedClonesList);
            }

            if (addDeSpawnedClones)
            {
                gameObjects.AddRange(deSpawnedClones);
            }
        }

        protected virtual void Awake()
        {
            if (Application.isPlaying)
            {
                PreloadAll();
                if (Persist)
                {
                    DontDestroyOnLoad(this);
                }
            }
        }

        protected virtual void OnEnable()
        {
            Instances.Add(this);
            RegisterPrefab();
        }

        protected virtual void OnDisable()
        {
            UnregisterPrefab();
            // 将池子从列表中移除
            Instances.Remove(this);
        }

        protected virtual void OnDestroy()
        {
            // 如果调用 OnDestroy，则场景可能会发生变化，因此我们将生成的预制件从全局链接字典中分离出来，以防止出现问题。
            foreach (var clone in spawnedClonesList)
            {
                if (clone != null)
                {
                    Pool.Detach(clone, false);
                }
            }
        }

        protected virtual void Update()
        {
            // 回收延迟回收对象的生命周期
            for (var i = delays.Count - 1; i >= 0; i--)
            {
                var delay = delays[i];
                delay.Life -= Time.deltaTime;
                if (delay.Life > 0)
                {
                    continue;
                }

                delays.RemoveAt(i);
                ClassPool<Delay>.Despawn(delay);

                if (delay.Clone != null)
                {
                    Despawn(delay.Clone);
                }
                else
                {
                    if (Warnings)
                    {
                        Debug.LogWarning(
                            "延迟回收对象所指向的克隆对象为空.",
                            this);
                    }
                }
            }
        }

        private void DespawnWithDelay(GameObject clone, float t)
        {
            for (var i = delays.Count - 1; i >= 0; i--)
            {
                var delay = delays[i];
                if (delay.Clone == clone)
                {
                    if (t < delay.Life)
                    {
                        delay.Life = t;
                    }

                    return;
                }
            }

            var newDelay = ClassPool<Delay>.Spawn() ?? new Delay();

            newDelay.Clone = clone;
            newDelay.Life = t;
            delays.Add(newDelay);
        }

        private void TryDeSpawn(GameObject clone)
        {
            if (spawnedClonesList.Remove(clone))
            {
                DespawnNow(clone);
            }
            else
            {
                if (Warnings)
                {
                    Debug.LogWarning(
                        "回收的对象不是此对象池生成的对象",
                        clone);
                }
            }
        }

        private void DespawnNow(GameObject clone, bool register = true)
        {
            // 是否加入到已回收对象的空闲列表中
            if (register)
            {
                deSpawnedClones.Add(clone);
            }

            if (clone.TryGetComponent<IPoolable>(out var comp))
            {
                comp.OnDespawn();
            }

            if (Strategy == StrategyType.ActivateAndDeactivate)
            {
                clone.SetActive(false);
                clone.transform.SetParent(transform, false);
            }
            else
            {
                clone.transform.SetParent(DeactivatedChild, false);
            }
        }

        private void SpawnClone(GameObject clone, Vector3 localPos, Quaternion localRotation, Vector3 localScale,
            Transform parent, bool worldPositionStay)
        {
            spawnedClonesList.Add(clone);

            var cloneTrans = clone.transform;
            cloneTrans.SetParent(null, false);

            cloneTrans.localPosition = localPos;
            cloneTrans.localRotation = localRotation;
            cloneTrans.localScale = localScale;

            cloneTrans.SetParent(parent, worldPositionStay);

            if (Strategy == StrategyType.ActivateAndDeactivate)
            {
                clone.SetActive(true);
            }

            if (clone.TryGetComponent<IPoolable>(out var com))
            {
                com.OnSpawn();
            }
        }

        private GameObject CreateClone(Vector3 localPosition, Quaternion localRotation, Vector3 localScale,
            Transform parent, bool worldPositionStays)
        {
            var clone = DoInstantiate(prefab, localPosition, localRotation, localScale, parent, worldPositionStays);

            if (Stamp)
            {
                clone.name = prefab.name + " " + Total;
            }
            else
            {
                clone.name = prefab.name;
            }

            return clone;
        }

        private GameObject DoInstantiate(GameObject prefabInstance, Vector3 localPosition, Quaternion localRotation,
            Vector3 localScale, Transform parent, bool worldPositionStays)
        {
            GameObject clone;
#if UNITY_EDITOR
            // 指定的对象是否是一个常规预制件（regular prefab）的一部分.
            // “常规预制件”指的是普通的，在Assets文件夹中可以直接找到的预制件（不是场景中特定实例化的预制件变体或链接到其他外部预制件）。
            if (!Application.isPlaying && PrefabUtility.IsPartOfRegularPrefab(prefabInstance))
            {
                clone = (GameObject)PrefabUtility.InstantiatePrefab(prefabInstance, parent);
                if (worldPositionStays)
                {
                    return clone;
                }

                clone.transform.localPosition = localPosition;
                clone.transform.localRotation = localRotation;
                clone.transform.localScale = localScale;
                return clone;
            }
#endif

            if (worldPositionStays)
            {
                return Instantiate(prefabInstance, parent, true);
            }

            clone = Instantiate(prefabInstance, localPosition, localRotation, parent);
            clone.transform.localPosition = localPosition;
            clone.transform.localRotation = localRotation;
            clone.transform.localScale = localScale;

            return clone;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/Pool/ObjectPool")]
        public static void AddPool()
        {
            new GameObject().AddComponent<ObjectPool>();
        }
#endif
    }
}