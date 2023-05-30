using UnityEngine;

namespace UFramework.GameCommon
{
    public interface IUIManager
    {
        T ShowView<T>(UILayer uiLayer, params object[] args) where T : class, IView, new();

        T CloseView<T>() where T : class, IView, new();

        void HideAll();

        void LocalUpdate(float dt);

        /// <summary>
        /// 获取当前View
        /// </summary>
        IView GetCurrentView();

        /// <summary>
        /// 获取当前View（不包括TipsViews）
        /// </summary>
        IView GetCurrentViewExcludeTip();

        /// <summary>
        /// 获取当前LowerView
        /// </summary>
        IView GetCurrentLowerView();

        /// <summary>
        /// 获取当前TopView
        /// </summary>
        IView GetCurrentTopView();

        /// <summary>
        /// 获取当前TipView
        /// </summary>
        IView GetCurrentTipView();
    }
}