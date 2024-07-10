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

    private ICoroutine coroutine;

    private void OnEnable()
    {
        // EventDispatcher();
        // MethodBind();
        coroutine = App.Make<ICoroutineManager>().StartCoroutine(WaitTime());
    }

    private bool flag;

    private void Update()
    {
        App.Make<ITweenManager>().LocalUpdate(Time.deltaTime);
        App.Make<ICoroutineManager>().LocalUpdate(Time.deltaTime);
        if (timer <= 0)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer > 0)
        {
            return;
        }
        
        // App.Make<ICoroutineManager>().StartCoroutine(OtherCoroutine1());
        // App.Make<ICoroutineManager>().StartCoroutine(OtherCoroutine2());
        if (!flag)
        {
            timer = 2;
            App.Make<ICoroutineManager>().StopCoroutine(coroutine);
            App.Make<ICoroutineManager>().StopCoroutine(coroutine);
            App.Make<ICoroutineManager>().StopCoroutine(coroutine);
            App.Make<ICoroutineManager>().StopCoroutine(coroutine);
        }
        else
        {
            App.Make<ICoroutineManager>().StartCoroutine(OtherCoroutine1());
        }

        flag = true;
    }

    #region Test

    private void EventDispatcher()
    {
        App.Make<IEventDispatcher>().AddListener("TestEvent", EventOne);
        App.Make<IEventDispatcher>().AddListener("TestEvent", EventTwo);
        App.Make<IEventDispatcher>().Raise("TestEvent", this, new EventParam("事件触发", "1").StopEvent());
        Facade<IEventDispatcher>.That.AddListener("TestInject", TestInject);
    }

    private void MethodBind()
    {
        App.BindMethod("MethodBind", (string param) =>
        {
            Debug.LogError($"测试函数绑定 参数:{param}");
            return null;
        });
        App.Bind<TestInject>();
        Facade<TestInject>.That.TestEvent();
    }

    private float timer = -1;

    private IEnumerator WaitTime()
    {
        yield return new UFramework.WaitForSeconds(1.0f);
        Debug.LogError("协程时间到了");
        timer = 2;
    }

    private IEnumerator OtherCoroutine1()
    {
        yield return new UFramework.WaitForSeconds(5f);
        Debug.LogError("Other1 Reached");
    }

    private IEnumerator OtherCoroutine2()
    {
        yield return new UFramework.WaitForSeconds(3f);
        Debug.LogError("Other2 Reached");
    }

    #endregion

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