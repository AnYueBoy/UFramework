using System;
using System.Collections;
using UFramework;
using UnityEngine;
using UApplication = UFramework.Application;

public class FrameworkTest : MonoBehaviour
{
    public UApplication application;

    private void Awake()
    {
        App.OnNewApplication += (IApplication application) => { Debug.Log("框架创建"); };
        application = UApplication.New();
        var systemBootstrap = new SystemProviderBootstrap(this);
        systemBootstrap.AddCustomProviders(new ProviderCoroutine(), new ProviderGameCommon(), new ProviderTweener());
        application.Bootstrap(
            systemBootstrap);

        Debug.Log($"runtimeId: {App.GetRuntimeId()}");

        application.Init();
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        App.Make<ICoroutineManager>().StartCoroutine(WaitTime());
    }

    private void Update()
    {
        App.Make<ICoroutineManager>().LocalUpdate(Time.deltaTime);
    }

    private IEnumerator WaitTime()
    {
        yield return new UFramework.WaitForSeconds(10.0f);
        Debug.LogError("协程时间到了");
    }

    private IEnumerator WaitTime2()
    {
        yield return new UFramework.WaitForSecondsRealtime(10.0f);
        Debug.LogError("真实时间 协程结束");
        App.Make<ICoroutineManager>().StartCoroutine(WaitTime()); 
    }
}