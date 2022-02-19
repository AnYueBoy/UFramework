/*
 * @Author: l hy 
 * @Date: 2022-01-04 16:02:40 
 * @Description: 链接
 */
using System;
using UnityEditor;
using UnityEngine;
public class Connection {

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public Action<Connection> onClickRemoveConnection;

    public Connection (ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> onClickRemoveConnection) {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.onClickRemoveConnection = onClickRemoveConnection;
    }

    public void Draw () {
        Handles.DrawBezier (
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f);

        if (Handles.Button ((inPoint.rect.center + outPoint.rect.center) / 2, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)) {
            onClickRemoveConnection?.Invoke (this);
        }

    }
}