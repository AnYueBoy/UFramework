using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Util {

    [MenuItem ("AFramework/isLandscape")]
    public static void isLandscape () {
        Util.clearConsole ();
        float aspect = (float) Screen.width / Screen.height;
        if (aspect > 1) {
            Debug.Log ("is landscape");
        } else {
            Debug.Log ("is not landscape");
        }

        Debug.Log ("aspece is : " + aspect);
    }

    public static void clearConsole () {
        Type log = typeof (EditorWindow).Assembly.GetType ("UnityEditor.LogEntries");

        var clearMethod = log.GetMethod ("Clear");
        clearMethod.Invoke (null, null);
    }
}