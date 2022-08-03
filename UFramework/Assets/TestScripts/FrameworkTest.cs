using UFramework.Bootstarp;
using UFramework.Core;
using UFramework.EventDispatcher;
using UnityEngine;

public class FrameworkTest : MonoBehaviour
{
    public UFramework.Core.Application application;

    private void Awake()
    {
        App.OnNewApplication += (IApplication application) => { Debug.Log("框架创建"); };
        application = UFramework.Core.Application.New();
        application.Bootstrap(new SystemProviderBootstrap(this));

        Debug.Log($"runtimeId: {App.GetRuntimeId()}");
    }

    private void Start()
    {
        application.Init();
    }

    private void OnEnable()
    {
        App.Make<IEventDispatcher>().AddListener("TestEvent", EventOne);
        App.Make<IEventDispatcher>().AddListener("TestEvent", EventTwo);
        App.Make<IEventDispatcher>().Raise("TestEvent", this, new EventParam("事件触发", "1").StopEvent());
    }

    private void EventOne(object sender, EventParam param)
    {
        Debug.LogWarning($"param{param.Value[0]} one");
    }

    private void EventTwo(object sender, EventParam param)
    {
        Debug.LogWarning($"param{param.Value[0]} two");
    }
}