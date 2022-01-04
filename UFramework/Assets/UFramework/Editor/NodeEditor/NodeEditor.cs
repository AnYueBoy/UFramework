using System.Collections.Generic;
using UFramework.AI.BehaviourTree;
using UnityEditor;
using UnityEngine;

public class NodeEditor : EditorWindow {
    private List<Node> nodes;
    private List<Connection> connections;
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;
    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;
    private Vector2 offset;
    private Vector2 drag;

    [MenuItem ("UFramework/NodeEditor")]
    private static void ShowWindow () {
        var window = GetWindow<NodeEditor> ();
        window.titleContent = new GUIContent ("NodeEditor");
        window.Show ();
    }

    private void OnEnable () {
        nodeStyle = new GUIStyle ();
        Texture2D texture = EditorGUIUtility.Load ("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.normal.background = texture;
        nodeStyle.border = new RectOffset (12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle ();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load ("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset (12, 12, 12, 12);

        inPointStyle = new GUIStyle ();
        inPointStyle.normal.background = EditorGUIUtility.Load ("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load ("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset (4, 4, 12, 12);

        outPointStyle = new GUIStyle ();
        outPointStyle.normal.background = EditorGUIUtility.Load ("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load ("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset (4, 4, 12, 12);
    }

    private void OnGUI () {
        this.drawGrid (20, 0.2f, Color.gray);
        this.drawGrid (100, 0.4f, Color.gray);
        this.drawNodes ();
        this.drawConnections ();
        this.drawConnectionLine (Event.current);
        this.processNodesEvents (Event.current);
        this.processEvents (Event.current);
        if (GUI.changed) Repaint ();
    }

    private void drawGrid (float gridSpacing, float gridOpacity, Color gridColor) {
        int widthDivs = Mathf.CeilToInt (position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt (position.height / gridSpacing);
        Handles.BeginGUI ();
        Handles.color = new Color (gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        offset += drag / 2;
        Vector3 newOffset = new Vector3 (offset.x % gridSpacing, offset.y % gridSpacing, 0);
        for (int i = 0; i < widthDivs; i++) {
            Handles.DrawLine (new Vector3 (gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3 (gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int i = 0; i < heightDivs; i++) {
            Handles.DrawLine (new Vector3 (-gridSpacing, gridSpacing * i, 0) + newOffset, new Vector3 (position.width, gridSpacing * i, 0) + newOffset);
        }
        Handles.color = Color.white;
        Handles.EndGUI ();
    }

    private void drawNodes () {
        if (this.nodes == null) {
            return;
        }

        for (int i = 0; i < nodes.Count; i++) {
            nodes[i].draw ();
        }
    }

    private void drawConnections () {
        if (connections == null) {
            return;
        }

        for (int i = 0; i < connections.Count; i++) {
            connections[i].draw ();
        }
    }

    private void drawConnectionLine (Event e) {
        if (selectedInPoint != null && selectedOutPoint == null) {
            Handles.DrawBezier (
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null) {
            Handles.DrawBezier (
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void processEvents (Event e) {
        drag = Vector2.zero;
        switch (e.type) {
            case EventType.MouseDown:
                if (e.button == 0) {
                    clearConnectionSelection ();
                }
                if (e.button == 1) {
                    this.processContextMenu (e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0) {
                    onDrag (e.delta);
                }
                break;
        }
    }

    private void onDrag (Vector2 delta) {
        drag = delta;
        if (nodes != null) {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].drag (delta);
            }
        }
        GUI.changed = true;
    }

    private void processNodesEvents (Event e) {
        if (nodes == null) {
            return;
        }

        for (int i = nodes.Count - 1; i >= 0; i--) {
            bool guiChanged = nodes[i].processEvents (e);
            if (guiChanged) {
                GUI.changed = true;
            }
        }
    }

    private void processContextMenu (Vector2 mousePosition) {
        GenericMenu genericMenu = new GenericMenu ();
        genericMenu.AddItem (new GUIContent ("Add Item"), false, () => onClickAddNode (mousePosition));
        genericMenu.ShowAsContext ();
    }

    private void onClickAddNode (Vector2 mousePosition) {
        if (nodes == null) {
            nodes = new List<Node> ();
        }
        nodes.Add (new Node (mousePosition, 100, 120, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onClickRemoveNode));
    }

    private void onClickInPoint (ConnectionPoint inPoint) {
        selectedInPoint = inPoint;
        if (selectedOutPoint == null) {
            return;
        }

        if (selectedOutPoint.node != selectedInPoint.node) {
            this.createConnection ();
        }
        this.clearConnectionSelection ();
    }
    private void onClickOutPoint (ConnectionPoint outPoint) {
        selectedOutPoint = outPoint;
        if (selectedInPoint == null) {
            return;
        }
        if (selectedOutPoint.node != selectedInPoint.node) {
            this.createConnection ();
        }
        this.clearConnectionSelection ();
    }

    private void onClickRemoveConnection (Connection connection) {
        connections.Remove (connection);
    }

    private void createConnection () {
        if (connections == null) {
            connections = new List<Connection> ();
        }

        connections.Add (new Connection (selectedInPoint, selectedOutPoint, onClickRemoveConnection));
    }

    private void clearConnectionSelection () {
        selectedInPoint = selectedOutPoint = null;
    }

    private void onClickRemoveNode (Node node) {
        nodes.Remove (node);

        if (connections == null) {
            return;
        }

        List<Connection> connectionsToRemove = new List<Connection> ();
        for (int i = 0; i < connections.Count; i++) {
            if (connections[i].inPoint == node.inPoint ||
                connections[i].outPoint == node.outPoint) {
                connectionsToRemove.Add (connections[i]);
            }
        }

        for (var i = 0; i < connectionsToRemove.Count; i++) {
            connections.Remove (connectionsToRemove[i]);
        }

        connectionsToRemove = null;
    }
}