using UnityEngine;
using Zenject;
public class IOCContainer : MonoBehaviour {

    private void Start () {
        this.bindManager ();
    }

    [Inject] private JectTwoManager jectTwoManager;
    private void bindManager () {
        DiContainer container = new DiContainer ();
        container.Bind<JectOneManager> ().AsSingle ();
        container.Bind<JectTwoManager> ().AsSingle ();

        container.Resolve<JectOneManager> ().connect (this.ToString ());

        container.Inject (this);

        jectTwoManager.connect ();
    }
}