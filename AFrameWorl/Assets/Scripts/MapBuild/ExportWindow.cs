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
                errorInfo = "当前关卡值不能为空";
                return;
            }

            int level = Int16.Parse (currentLevel);
            if (level < 0) {
                errorInfo = "当前关卡值不能小于0";
                return;
            }

            MapBuild.exprotJson (level);
            window.Close ();

            string openDir = Application.dataPath + "/MapConfig";
            Application.OpenURL ("file:///" + openDir);

        }
    }

}