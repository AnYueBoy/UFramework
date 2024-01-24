using UFramework.Core;
using UFramework.Core.Container;
using UFramework.EventDispatcher;

public class TestInject
{
    [Inject(Required = false)]
    public IEventDispatcher dispatcher { get; set; }

    public void TestEvent()
    {
        dispatcher.Raise("TestInject", this, new EventParam("Inject Success"));
        App.Invoke("MethodBind", "Are you ready");
    }
}