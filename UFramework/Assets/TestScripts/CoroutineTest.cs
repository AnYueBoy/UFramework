using System.Collections;
using UnityEngine;
using UFramework;

public class CoroutineTest : MonoBehaviour
{
    void Start()
    {
        App.Make<ICoroutineManager>().StartCoroutine(WaitTaskCoroutine());
        stopReference = App.Make<ICoroutineManager>().StartCoroutine(StopCoroutineTest());
    }

    private ICoroutine stopReference;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.LogError("执行停止");
            App.Make<ICoroutineManager>().StopCoroutine(stopReference);
        }
    }

    IEnumerator WaitCoroutine()
    {
        Debug.Log("协程开始");
        yield return new WaitForFrames(1);
        Debug.Log("等待一帧");
        yield return new UFramework.WaitForSeconds(3);
        Debug.Log("等待3秒");
    }

    IEnumerator WaitTaskCoroutine()
    {
        Debug.Log("等待开始");
        Debug.Log("等待测试");
        yield return new WaitForFrames(2);
        Debug.Log("等待结束");
    }

    IEnumerator StopCoroutineTest()
    {
        yield return new UFramework.WaitForSeconds(25f);

        Debug.LogError($"停止协程执行完毕");
    }
}