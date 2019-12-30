using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;

public class MapBuild : ScriptableWizard {
    // private void exprotJson () {

    //     Transform targetNode = GameObject.Find ("Obstacles").transform;

    //     NodeList nodeList = new NodeList ();

    //     nodeList.currentLevel = Int16.Parse (currentLevel);

    //     foreach (Transform transform in targetNode) {
    //         NodeInfo nodeInfo = new NodeInfo ();
    //         nodeInfo.nodeName = transform.name;
    //         nodeInfo.x = transform.position.x / 100;
    //         nodeInfo.y = transform.position.y / 100;
    //         nodeInfo.rotation = transform.rotation.z;
    //         nodeInfo.scale = transform.localScale.x;

    //         nodeList.nodeList.Add (nodeInfo);
    //     }

    //     string filePath = Application.dataPath + "/MapConfig" + "/MapConfig.json";

    //     FileInfo fileInfo = new FileInfo (filePath);
    //     if (!File.Exists (filePath)) {
    //         File.Delete (filePath);
    //     }

    //     StreamWriter sw = new StreamWriter (filePath);

    //     string jsonData = JsonMapper.ToJson (nodeList);

    //     sw.Write (jsonData);
    //     sw.Close ();

    //     AssetDatabase.Refresh ();
    // }

    [MenuItem ("Export/Exprot Json")]
    private static void createWindow () {
        MapBuild window = EditorWindow.GetWindow<MapBuild> ();
        window.Show ();
    }

    // private static MapBuild window = null;

    // private string currentLevel = "";

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
}