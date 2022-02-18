using UFramework.Core;
using UFramework.EventDispatcher;
using UnityEngine;
public class FrameworkTest : MonoBehaviour {

    public UFramework.Core.Application application;
    private void Awake () {
        application = UFramework.Core.Application.New ();
        application.Bootstrap (new ProviderBootStart ());
    }
    private void Start () {
        application.Init ();

        App.Make<ILogManager> ().printLog ();

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