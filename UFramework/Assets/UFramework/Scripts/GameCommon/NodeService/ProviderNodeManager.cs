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
        }
    }
}