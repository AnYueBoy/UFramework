/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:32:37 
 * @Description: 地图编辑
 */

namespace UFramework.Editor.MapBuild {
    using System.IO;
    using LitJson;
    using UFramework.Editor.EditorData;
    using UnityEditor;
    using UnityEngine;

    public class MapBuild : MonoBehaviour {
        public static string exprotJson (int currentLevel, string exportNodeName, string folderPath) {

            Transform targetNode = GameObject.Find (exportNodeName).transform;

            if (folderPath == "" || folderPath == null) {
                folderPath = Application.dataPath + "/MapConfig";
            }

            string filePath = folderPath + "/MapConfig.json";

            LevelMap levelMap = new LevelMap ();

            if (File.Exists (filePath)) {
                StreamReader sr = new StreamReader (filePath);
                string context = sr.ReadToEnd ();
                sr.Close ();
                levelMap = JsonMapper.ToObject<LevelMap> (context);
            }

            LevelInfo levelInfo = new LevelInfo ();

            levelInfo.currentLevel = currentLevel;

            foreach (Transform transform in targetNode) {
                NodeInfo nodeInfo = new NodeInfo ();
                transform.name = removeChar (transform.name, ' ');
                nodeInfo.nodeName = transform.name;
                nodeInfo.x = transform.position.x;
                nodeInfo.y = transform.position.y;
                nodeInfo.rotation = transform.eulerAngles.z;
                nodeInfo.scale = transform.localScale.x;

                levelInfo.nodeList.Add (nodeInfo);
            }

            int replaceIndex = -1;

            for (int i = 0; i < levelMap.levels.Count; i++) {
                LevelInfo level = levelMap.levels[i];
                if (level.currentLevel == currentLevel) {
                    replaceIndex = i;
                    break;
                }
            }

            if (replaceIndex != -1) {
                levelMap.levels[replaceIndex] = levelInfo;
            } else {
                levelMap.levels.Add (levelInfo);
            }

            // 当不存在文件时，流写入会自动创建文件
            StreamWriter sw = new StreamWriter (filePath);

            string jsonData = JsonMapper.ToJson (levelMap);

            sw.Write (jsonData);
            sw.Close ();

            AssetDatabase.Refresh ();
            return folderPath;
        }

        private static string removeChar (string nodeName, char remove) {
            int sliceIndex = -1;
            for (int i = 0; i < nodeName.Length; i++) {
                char character = nodeName[i];
                if (character.Equals (remove)) {
                    sliceIndex = i;
                    break;
                }
            }

            string result = nodeName;
            if (sliceIndex != -1) {
                result = nodeName.Substring (0, sliceIndex);
            }
            return result;
        }
    }
}