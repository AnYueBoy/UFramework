#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UFramework.GameCommon
{
    [ExecuteAlways]
    public class RuntimeComponent : MonoBehaviour
    {
        public BindData[] bindDataArray;

        [InitializeOnLoadMethod]
        private static void Load()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        private void OnEnable()
        {
            UpdateReference();
        }

        public void UpdateReference()
        {
            showBindDataList = bindDataArray;
        }

        private static BindData[] showBindDataList;

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (showBindDataList == null)
            {
                return;
            }

            int dataCount = showBindDataList.Length;

            for (int index = 0; index < dataCount; index++)
            {
                var data = showBindDataList[index];
                var bindObject = data.bindObject;
                if (bindObject.GetInstanceID() == instanceID)
                {
                    var r = new Rect(selectionRect);
                    r.x = 34;
                    r.width = 80;
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.yellow;
                    style.active.textColor = Color.red;
                    GUI.Label(r, "★", style);
                }
            }
        }
    }
}
#endif