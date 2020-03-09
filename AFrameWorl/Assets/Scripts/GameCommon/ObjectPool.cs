/*
 * @Author: l hy 
 * @Date: 2020-03-09 13:21:19 
 * @Description: 对象池 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-03-09 13:37:29
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {

    private static ObjectPool instance = null;

    private Dictionary<GameObject, List<GameObject>> pool = new Dictionary<GameObject, List<GameObject>> ();

    public static ObjectPool getInstance () {
        if (instance == null) {
            instance = new ObjectPool ();
        }
        return instance;
    }

    public GameObject requestInstance (GameObject prefab) {
        if (pool.ContainsKey (prefab)) {
            GameObject instance = null;
            List<GameObject> subPool = pool[prefab];
            for (int i = 0; i < subPool.Count; i++) {
                GameObject temp = subPool[i];
                if (!temp.activeSelf) {
                    instance = temp;
                    break;
                }
            }

            if (instance == null) {
                GameObject gameObject = GameObject.Instantiate<GameObject> (prefab);
                subPool.Add (gameObject);
                instance = gameObject;
            }

            instance.SetActive (true);
            return instance;
        } else {
            pool.Add (prefab, new List<GameObject> ());
            return requestInstance (prefab);
        }
    }

    public void returnInstance (GameObject target) {
        if (!target.activeSelf) {
            return;
        }

        if (pool.ContainsKey (target)) {
            List<GameObject> subPool = pool[target];
            for (int i = 0; i < subPool.Count; i++) {
                if (target == subPool[i]) {
                    subPool[i].SetActive (false);
                    return;
                }
            }
        }

        Debug.LogError("target"+target+"is not in pool");
    }

}