/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:31:59 
 * @Description: 导出窗口
 */
namespace UFramework.Editor.MapBuild {

    using System;
    using UnityEditor;
    using UnityEngine;

    public class ExportWindow : ScriptableWizard {

        private static ExportWindow window = null;

        [MenuItem ("UFramework/ExportWindow")]
        private static void exportJson () {
            window = EditorWindow.GetWindow<ExportWindow> ("导出json数据");
        }

        private string currentLevel = "";

        private string exportNodeName = "";

        private string folderPath = "";

        private void OnGUI () {
            GUILayout.Space (20.0f);
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("关卡值：", GUILayout.MaxWidth (120.0f));
            currentLevel = GUILayout.TextField (currentLevel);
            GUILayout.EndHorizontal ();

            GUILayout.Space (20.0f);
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("将要导出的节点名称", GUILayout.MaxWidth (120.0f));
            exportNodeName = GUILayout.TextField (exportNodeName);
            GUILayout.EndHorizontal ();

            GUILayout.Space (20.0f);
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("导出的文件目录", GUILayout.MaxWidth (120.0f));
            folderPath = GUILayout.TextField (folderPath);
            GUILayout.EndHorizontal ();

            GUILayout.Space (20.0f);
            if (GUILayout.Button ("导出")) {
                if (currentLevel == null || currentLevel == "") {
                    EditorUtility.DisplayDialog ("失败", "导出关卡值不能为空", "确认");
                    return;
                }

                int level = Int16.Parse (currentLevel);
                if (level < 0) {
                    EditorUtility.DisplayDialog ("失败", "关卡值不能小于0", "确认");
                    return;
                }

                if (this.exportNodeName == "" || this.exportNodeName == null) {
                    EditorUtility.DisplayDialog ("失败", "导出节点名称为空", "确认");
                    return;
                }

                try {
                    Transform targetNode = GameObject.Find (exportNodeName).transform;
                } catch (System.Exception) {
                    EditorUtility.DisplayDialog (
                        "失败",
                        "要导出的节点查找不到，请检查检点名称，当前节点名称：" + exportNodeName,
                        "确认");
                    return;
                }

                string resultPath = MapBuild.exprotJson (level, this.exportNodeName, folderPath);

                bool isOpenDis = EditorUtility.DisplayDialog ("成功", "是否打开文件所在文件夹", "确认", "取消");
                if (isOpenDis) {
                    Application.OpenURL ("file:///" + resultPath);
                }

            }
        }
    }
}