using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UFramework
{
    public static class UFrameworkSetting
    {
        [SettingsProvider]
        public static SettingsProvider UFrameworkSet()
        {
            var provider = new SettingsProvider("Project/UFrameworkSetting", SettingsScope.Project)
            {
                label = "基础配置",
                guiHandler = (searchContext) =>
                {
                    SerializedObject setting =
                        new SerializedObject(
                            AssetDatabase.LoadAssetAtPath<UFrameworkConfig>("Assets/UFrameworkConfig.asset"));

                    EditorGUILayout.PropertyField(setting.FindProperty("codeGeneratePath"), new GUIContent("UI代码生成路径"));
                    setting.ApplyModifiedPropertiesWithoutUndo();
                },
                keywords = new HashSet<string>(new[] { "codeGeneratePath" })
            };

            return provider;
        }
    }
}