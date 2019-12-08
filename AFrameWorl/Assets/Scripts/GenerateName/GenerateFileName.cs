using System;
using UnityEditor;
using UnityEngine;

/*
 * @Author: l hy 
 * @Date: 2019-12-08 14:39:05 
 * @Description: 生成文件时间 
 * @Last Modified by: l hy
 * @Last Modified time: 2019-12-08 14:39:27
 */

public class GenerateFileName {

    [MenuItem ("AFramework/generateFileTime")]
    private static void spawnFileTime () {
        Debug.Log ("AFramework_" + DateTime.Now.ToShortDateString ());
    }
}