using UnityEngine;

namespace UFramework.GameCommon
{
    public interface IUIManager
    {
        T ShowView<T>(UILayer uiLayer, params object[] args) where T : class, IView, new();

        T CloseView<T>() where T : class, IView, new();

        void HideAll();

        void LocalUpdate(float dt);
    }
}