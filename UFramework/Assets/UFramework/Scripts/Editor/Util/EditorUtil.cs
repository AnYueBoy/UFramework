/*
 * @Author: l hy 
 * @Date: 2020-05-12 08:38:46 
 * @Description: 编辑器工具类 
 */
namespace UFramework.Editor.Util {
    using System;
    using UnityEditor;
    public class EditorUtil {

        /// <summary>
        /// 清空控制台信息
        /// </summary>
        public static void clearConsole () {
            Type log = typeof (EditorWindow).Assembly.GetType ("UnityEditor.LogEntries");

            var clearMethod = log.GetMethod ("Clear");
            clearMethod.Invoke (null, null);
        }
    }
}