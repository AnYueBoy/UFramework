/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:33:10 
 * @Description: 地图恢复
 */

namespace UFramework.Editor.MapBuild {
    using System.IO;
    using LitJson;
    using UFramework.Editor.EditorData;
    using UnityEngine;

    public class MapRecovery : MonoBehaviour {

        public static string recovery (int levelValue, string recoveryNodeName, string folderPath) {
            string result = "还原成功";

            if (folderPath == "" || folderPath == null) {
                folderPath = Application.dataPath + "/MapConfig";
            }
            string filePath = folderPath + "/MapConfig.json";

            if (!File.Exists (filePath)) {
                result = "配置文件不存在,检查路径,当前路径:" + filePath;
                return result;
            }

            Transform targetNode = GameObject.Find (recoveryNodeName).transform;

            // 流数据读取
            StreamReader sr = new StreamReader (filePath);
            string context = sr.ReadToEnd ();
            sr.Close ();

            LevelMap levelMap = JsonMapper.ToObject<LevelMap> (context);

            while (targetNode.childCount > 0) {
                foreach (Transform obstacle in targetNode) {
                    GameObject.DestroyImmediate (obstacle.gameObject);
                }
            }

            LevelInfo levelInfo = null;

            for (int i = 0; i < levelMap.levels.Count; i++) {
                LevelInfo level = levelMap.levels[i];
                if (level.currentLevel == levelValue) {
                    levelInfo = level;
                    break;
                }
            }

            if (levelInfo == null) {
                result = "所要恢复的关卡数据不存在，请检查关卡值，当前关卡值：" + levelValue;
                return result;
            }

            for (int i = 0; i < levelInfo.nodeList.Count; i++) {
                NodeInfo nodeInfo = levelInfo.nodeList[i];

                GameObject orginGo = Resources.Load<GameObject> (nodeInfo.nodeName);
                GameObject node = GameObject.Instantiate (orginGo);
                node.name = nodeInfo.nodeName;

                node.transform.SetParent (targetNode);
                node.transform.position = new Vector3 ((float) nodeInfo.x, (float) nodeInfo.y, 0);
                node.transform.eulerAngles = new Vector3 (0, 0, (float) nodeInfo.rotation);
                node.transform.localScale = new Vector3 ((float) nodeInfo.scale, (float) nodeInfo.scale, (float) nodeInfo.scale);
            }

            return result;
        }
    }
}