using UnityEngine;

namespace UFramework.GameCommon
{
    public abstract class BindUI : MonoBehaviour
    {
        private RectTransform _rectTransform;

        public RectTransform RectTrans
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }
    }
}