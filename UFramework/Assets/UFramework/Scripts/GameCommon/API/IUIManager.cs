using UnityEngine;
namespace UFramework.GameCommon {

    public interface IUIManager {

        void Init (Transform uiRoot);
        
        void ShowBoard (string uiName, params object[] args);

        void ShowDialog (string uiName, params object[] args);

        void CloseDialog (string uiName);

        void HideAll ();
    }
}