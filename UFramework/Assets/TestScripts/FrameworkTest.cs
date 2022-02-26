using UFramework.Bootstarp;
using UFramework.Core;
using UFramework.EventDispatcher;
using UnityEngine;

public class FrameworkTest : MonoBehaviour {

    public UFramework.Core.Application application;
    private void Awake () {
        App.OnNewApplication += (IApplication application) => {
            Debug.Log ("框架创建");
        };
        application = UFramework.Core.Application.New ();
        application.Bootstrap (new SystemProviderBootstrap (this));

        Debug.Log ($"runtimeId: {App.GetRuntimeId()}");
    }
    private void Start () {
        application.Init ();

        var log1 = App.Make<LogManager> ("Are you ready?");
        var log2 = App.Make<LogManager> ("Log2");
        if (log1 == log2) {
            Debug.Log ("same instance");
        } else {
            Debug.Log ("not same instacne");
        }

        Debug.Log ($"id: {    UFramework.Core.Application.Version}");
    }

    private void OnEnable () {
        App.Make<IEventDispatcher> ().AddListener ("TestEvent", handlerEvent);
    }

    private void handlerEvent (object sender, EventParam e) {
        object value1 = e.value[0];
        Debug.Log ($"sender {sender} params: {e.value[0]}");
    }

    public void clickEvent () {
        App.Make<IEventDispatcher> ().Raise ("TestEvent", this, new EventParam (1, "12"));
    }

}