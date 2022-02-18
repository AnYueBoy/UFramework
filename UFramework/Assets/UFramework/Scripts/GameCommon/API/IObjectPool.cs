using UnityEngine;
namespace UFramework.GameCommon {

    public interface IObjectPool {
        GameObject requestInstance (GameObject prefab);

        void returnInstance (GameObject target);
    }
}