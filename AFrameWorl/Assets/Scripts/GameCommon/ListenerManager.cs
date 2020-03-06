using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * @Author: l hy 
 * @Date: 2020-03-05 08:57:49 
 * @Description: 事件监听 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-03-06 16:17:09
 */

public class ListenerManager {

    private static ListenerManager instance = null;

    public static ListenerManager getInstance () {
        if (instance == null) {
            instance = new ListenerManager ();
        }
        return instance;
    }
    public delegate void callBack (params object[] args);

    private Dictionary<string, Dictionary<object, callBack>> listenerMap = new Dictionary<string, Dictionary<object, callBack>> ();

    public void add (string eventName, object caller, callBack listener) {
        if (eventName == null || eventName == "") {
            Debug.LogError ("Listener eventName is null!");
            return;
        }

        if (caller == null) {
            Debug.LogError ("caller is null");
            return;
        }

        if (listener == null) {
            Debug.LogError ("listener is null");
            return;
        }

        Dictionary<object, callBack> listenerDic;
        if (this.listenerMap.ContainsKey (eventName)) {
            listenerDic = this.listenerMap[eventName];
        } else {
            listenerDic = new Dictionary<object, callBack> ();
            listenerMap.Add (eventName, listenerDic);
        }

        callBack listenerCall;
        if (listenerDic.ContainsKey (caller)) {
            listenerCall = listenerDic[caller];
            listenerCall += listener;
        } else {
            listenerDic.Add (caller, listener);
        }
    }

    public void remove (string eventName, object caller, callBack listener) {
        if (eventName == null || eventName == "") {
            Debug.LogError ("Listener eventName is null!");
            return;
        }

        if (caller == null) {
            Debug.LogError ("caller is null");
            return;
        }

        if (listener == null) {
            Debug.LogError ("listener is null");
            return;
        }
        Dictionary<object, callBack> listenerDic = null;
        if (this.listenerMap.ContainsKey (eventName)) {
            listenerDic = this.listenerMap[eventName];
        }

        if (listenerDic == null) {
            Debug.LogError ("remove fail not exist event: " + eventName);
            return;
        }

        callBack listenerCall = null;
        if (listenerDic.ContainsKey (caller)) {
            listenerCall = listenerDic[caller];
        }

        if (listenerCall == null) {
            Debug.LogError ("remove fail not exist caller: " + caller);
            return;
        }

        listenerCall -= listener;

    }

    public void removeAll (string eventName, object caller, callBack listener) {
        if (eventName == null || eventName == "") {
            Debug.LogError ("Listener eventName is null!");
            return;
        }

        if (caller == null) {
            Debug.LogError ("caller is null");
            return;
        }

        if (listener == null) {
            Debug.LogError ("listener is null");
            return;
        }
        Dictionary<object, callBack> listenerDic = null;
        if (this.listenerMap.ContainsKey (eventName)) {
            listenerDic = this.listenerMap[eventName];
        }

        if (listenerDic == null) {
            Debug.LogError ("remove fail not exist event: " + eventName);
            return;
        }

        if (listenerDic.ContainsKey (caller)) {
            listenerDic.Remove (caller);
        } else {
            Debug.LogError ("remove fail not exist caller: " + caller);
        }
    }

    public void trigger (string eventName, params object[] args) {
        if (eventName == null || eventName == "") {
            Debug.LogError ("not exist eventName: " + eventName);
            return;
        }

        Dictionary<object, callBack> listenerDic = null;
        if (listenerMap.ContainsKey (eventName)) {
            listenerDic = listenerMap[eventName];
        }

        if (listenerDic == null) {
            Debug.LogError ("not exist eventName: " + eventName);
            return;
        }

        foreach (callBack call in listenerDic.Values) {
            call (args);
        }
    }

}