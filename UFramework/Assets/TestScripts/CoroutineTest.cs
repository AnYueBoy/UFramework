using System;
using System.Collections;
using UFramework.Core;
using UFramework.Coroutine;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        App.Make<ICoroutineManager>().StartCoroutine(WaitCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator WaitCoroutine()
    {
        Debug.Log("协程开始");
        yield return new WaitForFrames(1);
        Debug.Log("等待一帧");
        yield return new UFramework.Coroutine.WaitForSeconds(3);
        Debug.Log("等待3秒");
    }
}