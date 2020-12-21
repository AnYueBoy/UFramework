/*
 * @Author: l hy 
 * @Date: 2019-12-08 14:39:05 
 * @Description: 导出unitypackage 包资源
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:46:56
 */
namespace UFramework.Editor.GenerateName {
    using System.IO;
    using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;

    public class GenerateFileName {

        [MenuItem ("UFramework/ExportPackage %e")]
        private static void callExportPackage () {
            exportPackage ();
        }

        // you can use this tag mean this function is obsolete
        //  [Obsolete ("this function is obsolete")]
        private static void exportPackage () {
            // 时间戳
            string timeStamp = DateTime.Now.ToString ("yyyyMMdd");
            string projectPath = Directory.GetParent (Application.dataPath).FullName;
            string packagePath = Directory.GetParent (projectPath).FullName;

            string filePathName = packagePath + @"\UFramework" + ".unitypackage";

            Debug.LogWarning ("path: " + filePathName);

            string assetPathName = "Assets/UFramework";

            // you can use this api let file name to copy board
            // GUIUtility.systemCopyBuffer = fileTime;

            AssetDatabase.ExportPackage (assetPathName, filePathName, ExportPackageOptions.Recurse);

            // open package floder
            Application.OpenURL ("file:///" + packagePath);
        }
    }
}