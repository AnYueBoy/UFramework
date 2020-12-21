/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:33:58 
 * @Description: 恢复窗口
 */
namespace UFramework.Editor.MapBuild {
    using System;
    using UnityEditor;
    using UnityEngine;

    public class RecoveryWindow : ScriptableWizard {

        private static RecoveryWindow window = null;

        [MenuItem ("UFramework/RecoveryWindow")]
        public static void recoveryNode () {
            window = EditorWindow.GetWindow<RecoveryWindow> (false, "恢复场景节点");
        }

        private string currentLevel = "";

        private string recoveryNodeName = "";

        private string folderPath = "";

        private void OnGUI () {
            GUILayout.Space (20.0f);
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("关卡值：", GUILayout.MaxWidth (120.0f));
            currentLevel = GUILayout.TextField (currentLevel);
            GUILayout.EndHorizontal ();

            GUILayout.Space (20.0f);
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("将要恢复的节点名称", GUILayout.MaxWidth (120.0f));
            recoveryNodeName = GUILayout.TextField (recoveryNodeName);
            GUILayout.EndHorizontal ();

            GUILayout.Space (20.0f);
            GUILayout.BeginHorizontal ();
            GUILayout.Label ("恢复的文件目录", GUILayout.MaxWidth (120.0f));
            folderPath = GUILayout.TextField (folderPath);
            GUILayout.EndHorizontal ();

            GUILayout.Space (20.0f);
            if (GUILayout.Button ("恢复")) {
                if (currentLevel == null || currentLevel == "") {
                    EditorUtility.DisplayDialog ("失败", "恢复关卡值不能为空", "确认");
                    return;
                }

                int level = Int16.Parse (currentLevel);
                if (level < 0) {
                    EditorUtility.DisplayDialog ("失败", "关卡值不能小于0", "确认");
                    return;
                }

                if (this.recoveryNodeName == "" || this.recoveryNodeName == null) {
                    EditorUtility.DisplayDialog ("失败", "恢复节点名称为空", "确认");
                    return;
                }

                try {
                    Transform targetNode = GameObject.Find (recoveryNodeName).transform;
                } catch (System.Exception) {
                    EditorUtility.DisplayDialog (
                        "失败",
                        "要导出的节点查找不到，请检查检点名称，当前节点名称：" + recoveryNodeName,
                        "确认");
                    return;
                }

                string result = MapRecovery.recovery (level, recoveryNodeName, folderPath);
                EditorUtility.DisplayDialog ("结果", result, "确认");
            }
        }
    }
}