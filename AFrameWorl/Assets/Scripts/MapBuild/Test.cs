using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : ScriptableWizard {
    private void OnGUI () {
        GUILayout.Label ("导出关卡配置文件");
        // this.currentLevel = GUILayout.TextField (this.currentLevel);
        if (GUILayout.Button ("导出")) {
            // if (this.currentLevel == "") {
            //     Debug.LogWarning ("请输入当前的关卡值");
            //     return;
            // }

            // exprotJson ();
            // MapBuild.window.Close ();
            // Application.OpenURL ("file:///" + );

        }
    }

    [MenuItem ("export/test")]
    private static void test () {
        Test testWindow = EditorWindow.GetWindow<Test> ();
        testWindow.Show ();
    }
}