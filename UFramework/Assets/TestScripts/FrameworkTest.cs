using UFramework.Core;
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
    }

}