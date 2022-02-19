/*
 * @Author: l hy 
 * @Date: 2022-01-04 14:03:53 
 * @Description: 链接点
 */
using System;
using UnityEngine;
public class ConnectionPoint {
    public Rect rect;
    public ConnectionPointType type;
    public Node node;

    public GUIStyle style;

    public Action<ConnectionPoint> onClickConnectionPoint;

    public ConnectionPoint (Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint) {
        this.node = node;
        this.type = type;
        this.style = style;
        this.onClickConnectionPoint = onClickConnectionPoint;
        this.rect = new Rect (0, 0, 10f, 20f);
    }

    public void Draw () {
        rect.y = node.rect.y + node.rect.height / 2 - rect.height / 2;

        switch (type) {
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + 8f;
                break;

            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - 8f;
                break;
            default:
                break;
        }

        if (GUI.Button (rect, "", style)) {
            onClickConnectionPoint?.Invoke (this);
        }

    }
}