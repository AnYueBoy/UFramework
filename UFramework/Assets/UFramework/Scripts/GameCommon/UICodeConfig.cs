using UnityEditor;
using UnityEngine;

namespace UFramework
{
    [CreateAssetMenu(fileName = "UFrameworkConfig", menuName = "ScriptableObjects / UFrameworkConfig", order = 1)]
    public class UFrameworkConfig : ScriptableObject
    {
        [Header("代码生成路径")] public string codeGeneratePath;

        public static UFrameworkConfig GetSerializedObject()
        {
            return AssetDatabase.LoadAssetAtPath<UFrameworkConfig>("Assets/UFrameworkConfig.asset");
        }
    }
}