using System.Collections;
using System.Collections.Generic;
using UFramework;
using UnityEngine;

public class ObjectPoolTest : MonoBehaviour
{
    [SerializeField] private GameObject prefab1;
    [SerializeField] private GameObject prefab2;

    void Start()
    {
        var node1 = Pool.Spawn(prefab1);
        node1.transform.SetParent(transform);
        node1.transform.position = Random.Range(0, 5f) * Vector3.one;
        var node2 = Pool.Spawn(prefab2);
        node2.transform.SetParent(transform);
        node2.transform.position = Random.Range(0, 5f) * Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
    }
}