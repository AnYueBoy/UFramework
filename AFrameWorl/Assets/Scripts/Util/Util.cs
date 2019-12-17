using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/*
 * @Author: l hy 
 * @Date: 2019-12-16 23:05:55 
 * @Description: 工具类
 * @Last Modified by: l hy 
 * @Last Modified time: 2019-12-16 23:05:55 
 */

public class Util {

    public static float getAspect () {
        float aspect = (float) Screen.width / Screen.height;
        return aspect;
    }

    public static void clearConsole () {
        Type log = typeof (EditorWindow).Assembly.GetType ("UnityEditor.LogEntries");

        var clearMethod = log.GetMethod ("Clear");
        clearMethod.Invoke (null, null);
    }

    public static bool isLandscape () {
        float aspect = getAspect ();
        if (aspect > 1) {
            return true;
        }

        return false;
    }

    public static bool isPadResoluation () {
        return targetValue (4 / 3.0f);
    }

    public static bool isPhone () {
        return targetValue (16 / 9.0f);
    }

    public static bool targetValue (float value) {
        float offset = 0.05f;
        float aspect = getAspect ();
        if (aspect > value - offset && aspect < value + offset) {
            return true;
        }
        return false;
    }
}