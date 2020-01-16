using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

public class MapRecovery : MonoBehaviour {

    public static string recovery (int level) {
        string result = "还原成功";
        string filePath = Application.dataPath + "/MapConfig" + "/MapConfig.json";
        if (!File.Exists (filePath)) {
            result = "配置文件不存在,检查路径,当前路径:" + filePath;
            return result;
        }

        // 流数据读取
        StreamReader sr = new StreamReader (filePath);
        string context = sr.ReadToEnd ();
        sr.Close ();

        NodeList nodeList = JsonMapper.ToObject<NodeList> (context);

        Transform obstacleTrans = GameObject.Find ("Obstacles").transform;
        while (obstacleTrans.childCount > 0) {
            foreach (Transform obstacle in obstacleTrans) {
                GameObject.DestroyImmediate (obstacle.gameObject);
            }
        }

        // for (int i = 0; i < nodeList)

        return result;
    }
}