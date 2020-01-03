using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;

public class MapBuild : MonoBehaviour {
    public static void exprotJson (int currentLevel) {
        if (currentLevel < 0) {
            Debug.Log ("level value is less than zero");
            return;
        }

        Transform targetNode = GameObject.Find ("Obstacles").transform;

        NodeList nodeList = new NodeList ();

        nodeList.currentLevel = currentLevel;

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

        // 当不存在文件时，流写入会自动创建文件
        StreamWriter sw = new StreamWriter (filePath);

        string jsonData = JsonMapper.ToJson (nodeList);

        sw.Write (jsonData);
        sw.Close ();

        AssetDatabase.Refresh ();
    }
}