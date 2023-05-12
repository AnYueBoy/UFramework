using UnityEngine;

namespace UFramework.GameCommon
{
    public interface IView
    {
        string ViewPath { get; }
        RectTransform ViewRect { get; }

        void OnShow(params object[] param);
    }
}