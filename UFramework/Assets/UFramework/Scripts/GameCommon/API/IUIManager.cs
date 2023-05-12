using UnityEngine;

namespace UFramework.GameCommon
{
    public interface IUIManager
    {
        void Init(RectTransform boardRoot, RectTransform dialogRoot);

        T ShowBoard<T>(params object[] args) where T : class, IView, new();

        T ShowDialog<T>(params object[] args) where T : class, IView, new();
        
        T CloseView<T>() where T : class, IView, new();

        void HideAll();
    }
}