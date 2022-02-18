using UnityEngine;
namespace UFramework.GameCommon {

    public interface IUIManager {

        void init (Transform uiRoot);
        
        void showBoard (string uiName, params object[] args);

        void showDialog (string uiName, params object[] args);

        void closeDialog (string uiName);

        void hideAll ();
    }
}