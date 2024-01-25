using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UFramework
{
    [CustomEditor(typeof(RuntimeComponent))]
    public class RuntimeComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty bindData;
        private ReorderableList bindDataList;
        private RuntimeComponent _runtimeComponent;

        private void OnEnable()
        {
            bindData = serializedObject.FindProperty("bindDataArray");
            bindDataList = new ReorderableList(serializedObject, bindData, true, true, true, true);

            bindDataList.drawHeaderCallback = DrawHeader;
            bindDataList.drawElementCallback = DrawListItems;
            bindDataList.onAddCallback = AddData;
            bindDataList.onRemoveCallback = RemoveData;
        }

        private ObjectInfo curOperateObject;
        private List<Rect> typeRectList;

        private static bool isReloadScriptCompleted;

        [DidReloadScripts]
        static void OnScriptReloadCompleted()
        {
            isReloadScriptCompleted = true;
        }

        public override void OnInspectorGUI()
        {
            OnDragUpdate();

            serializedObject.Update();
            bindDataList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("生成绑定代码"))
            {
                GenerateBindCode();
            }

            if (isReloadScriptCompleted && EditorPrefs.HasKey("ClassName"))
            {
                isReloadScriptCompleted = false;
                AddObjectReference();
            }
        }

        private void GenerateBindCode()
        {
            var context = serializedObject.targetObject as RuntimeComponent;
            var classUIName = context.gameObject.name + "UI";
            var classExtensionName = classUIName + "Extension";
            StringBuilder sb = new StringBuilder();
            HashSet<string> allNameSpace = new HashSet<string>();
            sb.Append("//此代码由程序自动生成切勿修改\n");

            sb.Append("public partial class " + classUIName + "\n");
            sb.Append("{\n");

            int dataCount = bindDataList.count;
            for (int i = 0; i < dataCount; i++)
            {
                SerializedProperty addData =
                    bindDataList.serializedProperty.GetArrayElementAtIndex(i);

                string variableName = addData.FindPropertyRelative("variableName").stringValue;
                object referenceObject = addData.FindPropertyRelative("bindComponent").objectReferenceValue;
                // 变量名称
                string variableTypeName = referenceObject.GetType().Name;
                // 变量名称所在命名空间
                string namespaceStr = referenceObject.GetType().Namespace;
                if (!string.IsNullOrEmpty(namespaceStr))
                {
                    allNameSpace.Add("using " + namespaceStr + ";\n");
                }

                sb.Append("\t public " + variableTypeName + " " + variableName + ";\n");
            }

            sb.Append("\n}");

            foreach (string namespaceStr in allNameSpace)
            {
                sb.Insert(0, namespaceStr);
            }

            string uiFilePath = UFrameworkConfig.GetSerializedObject().codeGeneratePath + classUIName + ".cs";
            if (!File.Exists(uiFilePath))
            {
                StringBuilder sbUI = new StringBuilder();
                sbUI.Append("using UnityEngine;\n");
                sbUI.Append("using UnityEngine.UI;\n");
                sbUI.Append("using UFramework.GameCommon;\n");

                sbUI.Append("public partial class " + classUIName + " : BindUI\n");
                sbUI.Append("{\n");
                sbUI.Append("\n}");
                File.WriteAllText(uiFilePath, sbUI.ToString());
            }

            string extensionFilePath =
                UFrameworkConfig.GetSerializedObject().codeGeneratePath + classExtensionName + ".cs";
            File.WriteAllText(extensionFilePath, sb.ToString());
            isReloadScriptCompleted = false;
            EditorPrefs.SetString("ClassName", classUIName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void AddObjectReference()
        {
            var classUIName = EditorPrefs.GetString("ClassName");
            EditorPrefs.DeleteKey("ClassName");
            var context = serializedObject.targetObject as RuntimeComponent;
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            Type type = assembly.GetType(classUIName);
            var component = context.gameObject.GetComponent(type);
            if (component != null)
            {
                DestroyImmediate(component);
            }

            component = context.gameObject.AddComponent(type);
            int dataCount = bindDataList.count;
            for (int i = 0; i < dataCount; i++)
            {
                SerializedProperty addData =
                    bindDataList.serializedProperty.GetArrayElementAtIndex(i);

                string variableName = addData.FindPropertyRelative("variableName").stringValue;
                object referenceObject = addData.FindPropertyRelative("bindComponent").objectReferenceValue;
                var componentType = component.GetType();
                var componentProperty =
                    componentType.GetField(variableName);
                componentProperty.SetValue(component, referenceObject);
            }

            EditorUtility.SetDirty(context);
        }

        private void OnDragUpdate()
        {
            Event e = Event.current;
            GUI.color = Color.green;
            //绘制一个监听区域
            var dragArea = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUIContent title = new GUIContent("拖动组件对象到此进行快速绑定");
            GUI.Box(dragArea, title);
            DrawTypes();

            switch (e.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (curOperateObject == null)
                    {
                        curOperateObject = BuildObjectInfo(DragAndDrop.objectReferences[0] as GameObject);
                    }

                    var index = GetContainsIndex(e.mousePosition);
                    if (index <= -1)
                    {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (e.type == EventType.DragPerform)
                    {
                        if (curOperateObject != null)
                        {
                            // 添加数据
                            int addIndex = bindDataList.count;
                            bindDataList.serializedProperty.InsertArrayElementAtIndex(addIndex);

                            SerializedProperty addData =
                                bindDataList.serializedProperty.GetArrayElementAtIndex(addIndex);
                            addData.FindPropertyRelative("variableName").stringValue = curOperateObject.gameObject.name;
                            addData.FindPropertyRelative("bindComponent").objectReferenceValue =
                                curOperateObject.components[index];
                            addData.FindPropertyRelative("bindObject").objectReferenceValue =
                                curOperateObject.gameObject;

                            serializedObject.ApplyModifiedProperties();
                            UpdateLinkReference();
                        }

                        DragAndDrop.AcceptDrag();
                    }

                    e.Use();
                    break;

                case EventType.DragExited:
                    curOperateObject = null;
                    typeRectList = new List<Rect>();
                    break;

                default:
                    break;
            }

            GUI.color = Color.white;
        }

        private void DrawTypes()
        {
            if (curOperateObject == null)
            {
                return;
            }

            GUILayout.BeginVertical();
            var allTypes = curOperateObject.types;

            typeRectList = new List<Rect>();
            for (var i = 0; i < allTypes.Length; i++)
            {
                var r = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                typeRectList.Add(r);
                GUI.color = r.Contains(Event.current.mousePosition) ? Color.green : Color.white;
                GUI.Box(r, allTypes[i]);
                Repaint();
            }

            GUILayout.EndVertical();
        }

        private int GetContainsIndex(Vector2 mousePos)
        {
            if (typeRectList == null || typeRectList.Count <= 0)
            {
                return -1;
            }

            for (int i = 0; i < typeRectList.Count; i++)
            {
                var rect = typeRectList[i];
                if (rect.Contains(mousePos))
                {
                    return i;
                }
            }

            return -1;
        }

        private ObjectInfo BuildObjectInfo(GameObject obj)
        {
            var info = new ObjectInfo();

            var types = new List<string>();
            var components = new List<Component>();

            info.gameObject = obj;

            var allComponent = obj.GetComponents(typeof(Component));

            foreach (var component in allComponent)
            {
                types.Add(component.GetType().Name);
                components.Add(component);
            }

            info.components = components.ToArray();
            info.types = types.ToArray();

            return info;
        }

        #region ReorderableList

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Components");
        }

        private readonly float itemWidth = 150;
        private readonly float itemInterval = 20;

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = bindDataList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, itemWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("variableName"), GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + itemWidth + itemInterval, rect.y, itemWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("bindComponent"), GUIContent.none);
        }

        private void AddData(ReorderableList list)
        {
            // 不允许通过手动添加数组中元素来进行组件的绑定

            // int addIndex = bindDataList.count;
            // bindDataList.serializedProperty.InsertArrayElementAtIndex(addIndex);
            // SerializedProperty addData =
            //     bindDataList.serializedProperty.GetArrayElementAtIndex(addIndex);
            // addData.FindPropertyRelative("variableName").stringValue = "";
            // addData.FindPropertyRelative("bindComponent").objectReferenceValue = null;
            // addData.FindPropertyRelative("bindObject").objectReferenceValue = null;
            // serializedObject.ApplyModifiedProperties();

            // UpdateLinkReference();
        }

        private void RemoveData(ReorderableList list)
        {
            int removeIndex = bindDataList.count - 1;
            bindDataList.serializedProperty.DeleteArrayElementAtIndex(removeIndex);
            serializedObject.ApplyModifiedProperties();
            UpdateLinkReference();
        }

        private void UpdateLinkReference()
        {
            var runtimeComp = serializedObject.targetObject as RuntimeComponent;
            var test = bindData;
            runtimeComp.UpdateReference();
        }

        #endregion

        private void OnDestroy()
        {
            // showBindDataList = null;
        }
    }


    [Serializable]
    public class BindUIData
    {
        public string variableName;

        // 绑定的组件
        public Object bindComponent;

        // 绑定组件所依赖的节点
        public Object bindObject;
    }

    public class ObjectInfo
    {
        public string[] types = { };
        public Component[] components = { };
        public GameObject gameObject;
    }
}