using UFramework.Core;
using UnityEngine;
public class ProviderTransform : MonoBehaviour, IServiceProvider {
    public void Init () {
        // App.Make<ITransformManager> ().GoTransform.gameObject.SetActive (false);
    }

    public void Register () {
        App.Instance<ITransformManager> (GetComponent<TransformManager> ());
    }
}