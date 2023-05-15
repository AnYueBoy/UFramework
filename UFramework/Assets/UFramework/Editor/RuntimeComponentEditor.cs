using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UFramework.GameCommon;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(RuntimeComponent))]
public class RuntimeComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OnDragUpdate();
    }

    private void OnDragUpdate()
    {
        Event e = Event.current;
        GUI.color = Color.green;
        //绘制一个监听区域
        var dragArea = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUIContent title = new GUIContent("拖动组件对象到此进行快速绑定");
        GUI.Box(dragArea, title);
        // DrawTypes();

        switch (e.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (e.type == EventType.DragPerform)
                {
                    var obj = DragAndDrop.objectReferences;
                    Debug.Log($"obj name: {obj[0].name}");
                    GameObject gameObject = obj[0] as GameObject;
                    var allComponent = gameObject.GetComponents(typeof(Component));

                    DragAndDrop.AcceptDrag();
                }

                e.Use();
                break;

            case EventType.DragExited:

                break;

            default:
                break;
        }

        GUI.color = Color.white;
    }

    private readonly Type[] allTypes = new Type[] { typeof(CanvasGroup), typeof(Image) };

    private List<Type> types = new List<Type>();

    private void DrawTypes()
    {
        GUILayout.BeginVertical();

        for (var i = 0; i < allTypes.Length; i++)
        {
            var r = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            // m_typeRects[i] = r;
            GUI.color = r.Contains(Event.current.mousePosition) ? Color.green : Color.white;
            GUI.Box(r, allTypes[i].Name);
            Repaint();
        }

        GUILayout.EndVertical();
    }


    [Button("生成绑定代码")]
    private void GenerateBindCode()
    {
    }
}