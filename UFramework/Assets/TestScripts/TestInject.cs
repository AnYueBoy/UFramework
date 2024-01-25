using UFramework;

public class TestInject
{
    /// <summary>
    /// 构造函数注入
    /// </summary>
    public TestInject(IEventDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    /// <summary>
    /// 属性注入
    /// </summary>
    [Inject]
    public IEventDispatcher dispatcher { get; set; }

    public void TestEvent()
    {
        dispatcher.Raise("TestInject", this, new EventParam("Inject Success"));
        App.Invoke("MethodBind", "Are you ready");
    }
}