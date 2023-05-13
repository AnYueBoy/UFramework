using UnityEngine;

namespace UFramework.GameCommon
{
    public class NodeManager : MonoBehaviour, INodeManager
    {
        [SerializeField] private RectTransform _lowerRoot;
        [SerializeField] private RectTransform _topRoot;
        [SerializeField] private RectTransform _tipRoot;

        public RectTransform LowerRoot => _lowerRoot;
        public RectTransform TopRoot => _topRoot;
        public RectTransform TipRoot => _tipRoot;
    }
}