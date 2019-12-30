using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;

public class MapBuild {

    [MenuItem ("Export/exprot node to json")]
    private static void exprotJson () {

        Transform targetNode = GameObject.Find ("Obstacles").transform;

        NodeList nodeList = new NodeList ();

        foreach (Transform transform in targetNode) {
            NodeInfo nodeInfo = new NodeInfo ();
            nodeInfo.nodeName = transform.name;
            nodeInfo.x = transform.position.x / 100;
            nodeInfo.y = transform.position.y / 100;
            nodeInfo.rotation = transform.rotation.z;
            nodeInfo.scale = transform.localScale.x;

            nodeList.nodeList.Add (nodeInfo);
        }

        string filePath = Application.dataPath + "/MapConfig" + "/MapConfig.json";

        FileInfo fileInfo = new FileInfo (filePath);
        if (!File.Exists (filePath)) {
            File.Delete (filePath);
        }

        StreamWriter sw = new StreamWriter (filePath);

        string jsonData = JsonMapper.ToJson (nodeList);

        sw.Write (jsonData);
        sw.Close ();

        AssetDatabase.Refresh ();
    }
}