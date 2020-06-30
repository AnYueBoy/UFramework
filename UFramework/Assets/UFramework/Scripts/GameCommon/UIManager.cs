/*
 * @Author: l hy 
 * @Date: 2020-03-07 17:13:07 
 * @Description: 界面管理器 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-03-08 21:57:27
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private static UIManager instance = null;

    private Dictionary<string, BaseUI> uiDic = new Dictionary<string, BaseUI> ();

    private BaseUI currentBoard = null;

    [Header ("ui根节点")]
    public GameObject uiRoot;

    public static UIManager getInstance () {
        return instance;
    }

    private void Awake () {
        if (instance == null) {
            instance = this;
        }

        this.currentBoard = null;
    }

    public void showBoard (string uiName, params object[] args) {
        BaseUI targerUI = this.getUI (uiName);
        if (currentBoard != null && this.currentBoard == targerUI) {
            return;
        }

        this.showUI (
            uiName,
            () => {
                if (this.currentBoard != null) {
                    this.hideUI (uiName);
                }

                this.currentBoard = this.getUI (uiName);
            },
            args);
    }

    public void hideAllDialog () {
        foreach (BaseUI targerUI in this.uiDic.Values) {
            if (targerUI != null && targerUI != this.currentBoard) {
                this.hideUI (targerUI.name);
            }
        }
    }

    public void showDialog (string uiName, params object[] args) {
        this.showUI (uiName, null, args);
    }

    private void hideUI (string uiName) {
        BaseUI targetUI = this.getUI (uiName);
        if (targetUI != null) {
            targetUI.gameObject.SetActive (false);
        }
    }

    private BaseUI getUI (string uiName) {
        if (this.uiDic.ContainsKey (uiName)) {
            return this.uiDic[uiName];
        }

        return null;
    }

    private void showUI (string uiName, Action callBack = null, params object[] args) {
        BaseUI targetUI = this.getUI (uiName);
        if (targetUI != null) {
            targetUI.gameObject.SetActive (true);
            if (callBack != null) {
                callBack ();
            }

            targetUI.onShow (args);
        } else {
            this.openUI (
                uiName,
                () => {
                    if (callBack != null) {
                        callBack ();
                    }

                    BaseUI ui = this.getUI (uiName);
                    ui.onShow (args);
                }
            );
        }
    }

    private void openUI (string uiName, Action callBack) {
        string url = UrlString.uiUrl + uiName;
        GameObject prefab = Resources.Load<GameObject> (url);

        GameObject uiNode = GameObject.Instantiate (prefab);
        uiNode.transform.SetParent (this.uiRoot.transform);
        uiNode.transform.localPosition = Vector3.zero;
        BaseUI targetUI = uiNode.GetComponent<BaseUI> ();
        this.uiDic.Add (uiName, targetUI);
        if (callBack != null) {
            callBack ();
        }
    }
}