/*
 * @Author: l hy 
 * @Date: 2021-01-27 10:09:38 
 * @Description: GUI测试脚本
 */
using System;
using UFramework.EventDispatcher;
using UnityEngine;
public class GUITest : MonoBehaviour {
    private Transform transformSelf;

    private void OnGUI () {
        GUILayout.BeginHorizontal ();
        if (GUILayout.Button ("按钮")) {

        }
        GUILayout.EndHorizontal ();

        var eventDispatcher = new EventDispatcher ();
        // eventDispatcher.Raise ("", this, new GeneralArgs (1));

        // eventDispatcher.AddListener ("", listener);
    }

    private void listener (object sender, GeneralArgs e) { }

}

public class GeneralArgs : EventArgs {
    public object value;

    public GeneralArgs (object value = null) {
        this.value = value;
    }
}

// public delegate void EventHandler(object sender,GeneralArgs e);