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

        private static Texture2D linkTexture;

        private void OnEnable()
        {
            UpdateReference();
            linkTexture = EditorGUIUtility.IconContent("d_UnityEditor.FindDependencies").image as Texture2D;
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
            var originColor = GUI.color;

            GUI.color = Color.green;
            for (int index = 0; index < dataCount; index++)
            {
                var data = showBindDataList[index];
                var bindObject = data.bindObject;
                if (bindObject.GetInstanceID() == instanceID)
                {
                    var r = new Rect(selectionRect);
                    r.x = 35;
                    r.width = 16;
                    r.height = 16;
                    GUI.DrawTexture(r, linkTexture);
                }
            }
            
            GUI.color = originColor;

        }
    }
}
#endif