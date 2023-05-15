#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;

namespace UFramework.GameCommon
{
    public class RuntimeComponent : MonoBehaviour
    {
        [SerializeField] private Text varName;
        private RectTransform _rectTrans;

        public RectTransform RectTrans => _rectTrans;

        public void Init()
        {
            _rectTrans = GetComponent<RectTransform>();
        }

        public BindData[] bindDataArray;
    }
}
#endif