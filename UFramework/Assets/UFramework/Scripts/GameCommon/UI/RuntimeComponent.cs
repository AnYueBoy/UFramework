using UnityEngine;

namespace UFramework.GameCommon
{
    public class RuntimeComponent : MonoBehaviour
    {
        private RectTransform _rectTrans;

        public RectTransform RectTrans => _rectTrans;

        public void Init()
        {
            _rectTrans = GetComponent<RectTransform>();
        }
        
     
    }
}