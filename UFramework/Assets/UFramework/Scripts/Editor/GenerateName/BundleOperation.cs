﻿/*
 * @Author: l hy 
 * @Date: 2021-03-01 21:42:21 
 * @Description: AB包操作
 * @Last Modified by: l hy
 * @Last Modified time: 2021-03-01 21:58:10
 */
namespace UFramework.Editor.GenerateName {
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
#endif

    public class BundleOperation {
        [MenuItem ("UFramework/BuildBundle")]
        private static void buildAssetBundle () {
            string bundleUrl = Application.dataPath + "/AssetsBundles";
            if (!Directory.Exists (bundleUrl)) {
                Directory.CreateDirectory (bundleUrl);
            }
            BuildPipeline.BuildAssetBundles (bundleUrl, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            Application.OpenURL ("file:///" + bundleUrl);
        }

    }
}