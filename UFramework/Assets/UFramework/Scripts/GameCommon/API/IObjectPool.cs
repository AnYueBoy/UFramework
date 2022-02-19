using UnityEngine;
namespace UFramework.GameCommon {

    public interface IObjectPool {
        GameObject RequestInstance (GameObject prefab);

        void ReturnInstance (GameObject target);
    }
}