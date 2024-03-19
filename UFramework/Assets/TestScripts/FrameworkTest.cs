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
        App.Make<IUIManager>().ShowView<GameBoard>(UILayer.Lower);
    }

    private void OnEnable()
    {
        App.Make<IEventDispatcher>().AddListener("TestEvent", EventOne);
        App.Make<IEventDispatcher>().AddListener("TestEvent", EventTwo);
        App.Make<IEventDispatcher>().Raise("TestEvent", this, new EventParam("事件触发", "1").StopEvent());
        Facade<IEventDispatcher>.That.AddListener("TestInject", TestInject);
        App.BindMethod("MethodBind", (string param) =>
        {
            Debug.LogError($"测试函数绑定 参数:{param}");
            return null;
        });
        App.Bind<TestInject>();
        Facade<TestInject>.That.TestEvent();
    }

    private void Update()
    {
        App.Make<ITweenManager>().LocalUpdate(Time.deltaTime);
        App.Make<ICoroutineManager>().LocalUpdate(Time.deltaTime);
    }

    private void EventOne(object sender, EventParam param)
    {
        Debug.LogWarning($"param{param.Value[0]} one");
    }

    private void EventTwo(object sender, EventParam param)
    {
        Debug.LogWarning($"param{param.Value[0]} two");
    }

    private void TestInject(object sender, EventParam param)
    {
        Debug.LogError($"{param.Value[0]}");
    }
}