using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/*
 * @Author: l hy 
 * @Date: 2019-12-08 14:39:05 
 * @Description: 导出unitypackage 包资源
 * @Last Modified by: l hy
 * @Last Modified time: 2020-04-10 22:48:36
 */

public class GenerateFileName {

    [MenuItem ("AFramework/ExportPackage %e")]
    private static void callExportPackage () {
        exportPackage ();
    }

    // you can use this tag mean this function is obsolete
    //  [Obsolete ("this function is obsolete")]
    private static void exportPackage () {
        // 时间戳
        string timeStamp = DateTime.Now.ToString ("yyyyMMdd");
        string filePathName = "D:/UnityWork/AFrameWork/" + "AFramework" + ".unitypackage";
        string floderPath = "D:/UnityWork/AFrameWork/";

        string assetPathName = "Assets/AFramework";

        // you can use this api let file name to copy board
        // GUIUtility.systemCopyBuffer = fileTime;

        AssetDatabase.ExportPackage (assetPathName, filePathName, ExportPackageOptions.Recurse);

        // open package floder
        Application.OpenURL ("file:///" + floderPath);
    }
}