/*
 * @Author: l hy 
 * @Date: 2022-01-04 10:03:17 
 * @Description: 节点
 */

using System;
using UnityEditor;
using UnityEngine;
public class Node {
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;
    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    public Action<Node> onRemoveNode;

    public Node (
        Vector2 postion,
        float width,
        float height,
        GUIStyle nodeStyle,
        GUIStyle selectedStyle,
        GUIStyle inPointStyle,
        GUIStyle outPointStyle,
        Action<ConnectionPoint> onClickInPoint,
        Action<ConnectionPoint> onClickOutPoint,
        Action<Node> onRemoveNode) {
        rect = new Rect (postion.x, postion.y, width, height);
        this.style = nodeStyle;
        defaultNodeStyle = nodeStyle;
        this.selectedNodeStyle = selectedStyle;
        this.inPoint = new ConnectionPoint (this, ConnectionPointType.In, inPointStyle, onClickInPoint);
        this.outPoint = new ConnectionPoint (this, ConnectionPointType.Out, outPointStyle, onClickOutPoint);
        this.onRemoveNode = onRemoveNode;
    }

    public void drag (Vector2 delta) {
        rect.position += delta;
    }
    public void draw () {
        inPoint.draw ();
        outPoint.draw ();
        GUI.Box (rect, title, style);
    }

    public bool processEvents (Event e) {
        switch (e.type) {
            case EventType.MouseDown:
                if (e.button == 0) {
                    if (rect.Contains (e.mousePosition)) {
                        this.isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    } else {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains (e.mousePosition)) {
                    processContextMenu ();
                    e.Use ();
                }
                break;

            case EventType.MouseUp:
                this.isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && this.isDragged) {
                    this.drag (e.delta);
                    e.Use ();
                    return true;
                }
                break;
        }
        return false;
    }

    private void processContextMenu () {
        GenericMenu genericMenu = new GenericMenu ();
        genericMenu.AddItem (new GUIContent ("Remove Node"), false, onClickRemoveNode);
        genericMenu.ShowAsContext ();
    }

    private void onClickRemoveNode () {
        onRemoveNode?.Invoke (this);
    }
}