using UFramework.Core;
using UnityEngine;

namespace UFramework.GameCommon
{
    public class ProviderNodeManager : MonoBehaviour, IServiceProvider
    {
        public void Register()
        {
            App.Instance<INodeManager>(GetComponent<NodeManager>());
        }

        public void Init()
        {
            var nodeManager = App.Make<INodeManager>();
            var root = nodeManager.LowerRoot;
            Debug.Log("");
        }
    }
}