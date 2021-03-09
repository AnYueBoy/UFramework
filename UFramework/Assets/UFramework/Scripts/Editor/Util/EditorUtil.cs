/*
 * @Author: l hy 
 * @Date: 2020-05-12 08:38:46 
 * @Description: 编辑器工具类 
 */
namespace UFramework.Editor.Util {
    using System;
    using UnityEditor;
    using UnityEngine;

    public class EditorUtil {

        /// <summary>
        /// 清空控制台信息
        /// </summary>
        public static void clearConsole () {
            Type log = typeof (EditorWindow).Assembly.GetType ("UnityEditor.LogEntries");

            var clearMethod = log.GetMethod ("Clear");
            clearMethod.Invoke (null, null);
        }

        /// <summary>
        /// 获取当前平台名称
        /// </summary>
        /// <returns></returns>
        public static string getCurPlatformName () {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            switch (buildTarget) {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";

                case BuildTarget.iOS:
                    return "IOS";

                case BuildTarget.Android:
                    return "Android";

                case BuildTarget.StandaloneLinux64:
                    return "Linux";

                default:
                    // FIXME: other platfrom not support
                    return null;
            }

        }

        public static string getBuildBundleUrl () {
            string platformName = getCurPlatformName ();
            return Application.dataPath + "/AssetsBundles/" + platformName + "/";
        }
    }
}