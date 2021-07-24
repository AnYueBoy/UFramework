using UnityEngine;
using Zenject;

public class ZenjectInstaller : MonoInstaller {
    public override void InstallBindings () {
        Container.Bind<JectOneManager> ().AsSingle ();

        Container.Bind<JectTwoManager> ().AsSingle ();
    }
}