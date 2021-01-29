/*
 * @Author: l hy 
 * @Date: 2021-01-27 10:09:38 
 * @Description: GUI测试脚本
 */
using UnityEngine;
public class GUITest : MonoBehaviour {

    private void OnGUI () {
        GUILayout.BeginHorizontal ();
        if (GUILayout.Button ("按钮")) {

        }
        GUILayout.EndHorizontal ();
    }
}