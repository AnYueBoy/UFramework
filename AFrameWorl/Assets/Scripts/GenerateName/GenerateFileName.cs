using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * @Author: l hy 
 * @Date: 2019-12-08 14:39:05 
 * @Description: 导出unitypackage 包资源
 * @Last Modified by: l hy
 * @Last Modified time: 2019-12-08 17:02:32
 */

public class GenerateFileName {

    [MenuItem ("AFramework/exportPackage %e")]
    private static void callExportPackage () {
        exportPackage ();
    }

    public static void exportPackage () {
        string filePathName = "D:/UnityWork/AFrameWork/" + "AFramework_" + DateTime.Now.ToString ("yyyyMMdd") + ".unitypackage";
        string floderPath = "D:/UnityWork/AFrameWork/";

        string assetPathName = "Assets/Scripts";

        // you can use this api let file name to copy board
        // GUIUtility.systemCopyBuffer = fileTime;

        AssetDatabase.ExportPackage (assetPathName, filePathName, ExportPackageOptions.Recurse);

        // open package floder
        Application.OpenURL ("file:///" + floderPath);

    }
}