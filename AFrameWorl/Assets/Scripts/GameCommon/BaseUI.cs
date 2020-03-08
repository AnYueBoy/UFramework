/*
 * @Author: l hy 
 * @Date: 2020-03-07 16:37:25 
 * @Description: 界面基类 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-03-08 22:44:30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour {

    protected static string resUrl = "BaseUI";

    private void Awake () {
        this.myTag = this;
    }

    private BaseUI myTag;

    public BaseUI Tag {
        get {
            return this.myTag;
        }
    }

    public string getUrl () {
        return UrlString.uiUrl + resUrl;
    }

    public void onShow (params object[] args) {

    }
}