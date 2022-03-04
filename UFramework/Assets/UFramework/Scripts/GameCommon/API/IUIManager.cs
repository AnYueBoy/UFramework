using UnityEngine;
namespace UFramework.GameCommon {

    public interface IUIManager {

        void Init (Transform uiRoot);

        void ShowBoard (string uiName, params object[] args);

        void ShowBoard<T> (params object[] args) where T : BaseUI;

        void ShowDialog (string uiName, params object[] args);

        void ShowDialog<T> (params object[] args) where T : BaseUI;

        void CloseDialog (string uiName);

        void CloseDialog<T> () where T : BaseUI;

        void HideAll ();
    }
}