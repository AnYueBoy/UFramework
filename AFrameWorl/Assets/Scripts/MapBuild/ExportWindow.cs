using System;
using UnityEditor;
using UnityEngine;

public class ExportWindow : ScriptableWizard {

    private static ExportWindow window = null;

    [MenuItem ("AFramework/ExportWindow")]
    private static void exportJson () {
        window = ScriptableWizard.DisplayWizard<ExportWindow> ("导出json数据");
    }

    private string currentLevel = "";

    private string errorInfo = "";
    private void OnGUI () {
        currentLevel = GUILayout.TextField (currentLevel);
        GUILayout.Label (errorInfo);
        if (GUILayout.Button ("导出")) {
            if (currentLevel == null || currentLevel == "") {
                EditorUtility.DisplayDialog("失败","导出关卡值不能为空","确认");
                return;
            }

            int level = Int16.Parse (currentLevel);
            if (level < 0) {
                EditorUtility.DisplayDialog("失败","关卡值不能小于0","确认");
                return;
            }

            MapBuild.exprotJson (level);
            window.Close ();

            bool isOpenDis = EditorUtility.DisplayDialog ("成功", "是否打开文件所在文件夹", "确认", "取消");
            if (isOpenDis) {
                string openDir = Application.dataPath + "/MapConfig";
                Application.OpenURL ("file:///" + openDir);
            }

        }
    }

}