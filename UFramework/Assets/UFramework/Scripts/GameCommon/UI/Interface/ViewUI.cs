using UnityEngine;

namespace UFramework.GameCommon
{
    public abstract class ViewUI : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public RectTransform RectTrans => _rectTransform;

        public void Init()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }
}