using System;
using UnityEditor;
using UnityEngine;

/*
 * @Author: l hy 
 * @Date: 2019-12-08 14:39:05 
 * @Description: 生成文件名到剪切板
 * @Last Modified by: l hy
 * @Last Modified time: 2019-12-08 14:56:19
 */

public class GenerateFileName {

    [MenuItem ("AFramework/generateFileTime")]
    private static void spawnFileTime () {
        string fileTime = "AFramework_" + DateTime.Now.ToString ("yyyyMMdd");
        GUIUtility.systemCopyBuffer = fileTime;
        Debug.Log ("generate success");
    }
}