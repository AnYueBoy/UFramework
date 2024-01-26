/*
 * @Author: l hy
 * @Date: 2019-12-08 14:39:05
 * @Description: 导出unitypackage 包资源
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:46:56
 */

using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using SApplication = UnityEngine.Application;

namespace UFramework
{
    public class GenerateFileName
    {
        [MenuItem("UFramework/ExportPackage %e")]
        private static void ExportPackage()
        {
            // 时间戳
            string timeStamp = DateTime.Now.ToString("yyyyMMdd");
            string projectPath = Directory.GetParent(SApplication.dataPath).FullName;
            string packagePath = Directory.GetParent(projectPath).FullName + "/";
            string dllPackagePath = packagePath + "Core/";

            string filePathName = packagePath + "UFramework.unitypackage";

            string assetPathName = "Assets/UFramework";

            // you can use this api let file name to copy board
            // GUIUtility.systemCopyBuffer = fileTime;

            AssetDatabase.ExportPackage(assetPathName, filePathName, ExportPackageOptions.Recurse);

            string dllFileFolder = projectPath + "/Library/ScriptAssemblies/";
            string uframeworkDLL = dllFileFolder + "UFramework.dll";
            string uframeworkDestDll = dllPackagePath + "UFramework.dll";

            var uframeworkAssembly =
                AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>("Assets/UFramework/UFramework.asmdef");
            Asmdef asmdef = JsonUtility.FromJson<Asmdef>(uframeworkAssembly.text);

            foreach (var reference in asmdef.references)
            {
                string guid = reference.Substring(5, reference.Length - 5);
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var startIndex = path.LastIndexOf("/");
                var endIndex = path.LastIndexOf(".");
                var dllName = path.Substring(startIndex + 1, endIndex - startIndex - 1) + ".dll";
                var destDllPath = dllPackagePath + dllName;
                var sourceDllPath = dllFileFolder + dllName;
                if (!Directory.Exists(dllPackagePath))
                {
                    Directory.CreateDirectory(dllPackagePath);
                }

                if (File.Exists(destDllPath))
                {
                    File.Delete(destDllPath);
                }

                File.Copy(sourceDllPath, destDllPath);
            }

            if (File.Exists(uframeworkDLL))
            {
                File.Delete(uframeworkDestDll);
            }

            File.Copy(uframeworkDLL, uframeworkDestDll);

            // open package floder
            SApplication.OpenURL("file:///" + packagePath);

            Debug.Log("导出成功");
        }

        [Serializable]
        private class Asmdef
        {
            public string name;
            public string[] references;
        }
    }
}