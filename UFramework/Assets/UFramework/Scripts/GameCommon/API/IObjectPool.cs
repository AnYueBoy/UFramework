using UnityEngine;

namespace UFramework
{
    public interface IObjectPool
    {
        GameObject RequestInstance(GameObject prefab);

        void ReturnInstance(GameObject target);
    }
}