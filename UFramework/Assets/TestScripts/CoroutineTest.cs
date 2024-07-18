using System.Collections;
using UnityEngine;
using UFramework;
using WaitForSeconds = UFramework.WaitForSeconds;

public class CoroutineTest : MonoBehaviour
{
    void Start()
    {
        App.Make<ICoroutineManager>().StartCoroutine(WaitTime());
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(10f);
        Debug.LogError($"协程结束");
    }
}