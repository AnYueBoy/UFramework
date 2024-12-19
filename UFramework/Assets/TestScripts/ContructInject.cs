using System.Collections;
using System.Collections.Generic;
using UFramework;
using UnityEngine;
using UApplication = UFramework.Application;

public class ContructInject : MonoBehaviour
{
    public UApplication application;

    private void Awake()
    {
        App.OnNewApplication += (IApplication application) => { Debug.Log("框架创建"); };
        application = UApplication.New();
        var systemBootstrap = new SystemProviderBootstrap(this);
        systemBootstrap.AddCustomProviders(new ProviderCoroutine(), new ProviderContructInject());
        application.Bootstrap(
            systemBootstrap);

        Debug.Log($"runtimeId: {App.GetRuntimeId()}");

        application.Init();
    }
}