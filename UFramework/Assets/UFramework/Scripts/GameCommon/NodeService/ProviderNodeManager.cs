using UnityEngine;

namespace UFramework
{
    public class ProviderNodeManager : MonoBehaviour, IServiceProvider
    {
        public void Register()
        {
            App.Instance<INodeManager>(GetComponent<NodeManager>());
        }

        public void Init()
        {
        }
    }
}