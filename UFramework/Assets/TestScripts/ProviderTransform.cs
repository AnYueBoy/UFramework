using UFramework.Core;
using UnityEngine;
public class ProviderTransform : IServiceProvider {
    public void Init () {
        // ITransformManager transformManager = App.Make<ITransformManager> ();
        // Transform selfTranform = transformManager.GoTransform;
        // selfTranform.gameObject.SetActive(false);
    }

    public void Register () {
        App.Singleton<ITransformManager, TransformManager> ();
    }
}