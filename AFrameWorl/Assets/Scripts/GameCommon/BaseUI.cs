/*
 * @Author: l hy 
 * @Date: 2020-03-07 16:37:25 
 * @Description: 界面基类 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-03-07 17:11:31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour {

    protected static string resUrl = "BaseUI";

    private object myTag;

    public object tag {
        get {
            return this.myTag;
        }

        set {
            this.myTag = value;
        }
    }

    public static string getUrl () {
        return "" + resUrl;
    }

    public void onShow (params object[] args) {

    }
}