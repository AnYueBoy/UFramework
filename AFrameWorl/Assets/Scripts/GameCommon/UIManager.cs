using System;
/*
 * @Author: l hy 
 * @Date: 2020-03-07 17:13:07 
 * @Description: 界面管理器 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-03-08 21:57:27
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private static UIManager instance = null;

    private List<BaseUI> uiList = new List<BaseUI> ();

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

    public void showBoard (BaseUI ui, params object[] args) {
        BaseUI bUI = this.getUI (ui);
        if (currentBoard != null && this.currentBoard == ui) {
            return;
        }

        this.showUI (
            ui,
            () => {
                if (this.currentBoard != null) {
                    this.hideUI (this.currentBoard.Tag);
                }
                this.currentBoard = this.getUI (ui);
            },
            args);
    }

    public void hideAllDialog () {
        for (int i = 0; i < this.uiList.Count; i++) {
            BaseUI bUI = this.uiList[i];
            if (bUI != null && bUI != this.currentBoard) {
                this.hideUI (bUI.Tag);
            }
        }
    }

    public void showDialog (BaseUI ui, params object[] args) {
        this.showUI (ui, null, args);
    }

    private void hideUI (BaseUI ui) {
        BaseUI bUI = this.getUI (ui);
        if (ui != null) {
            ui.gameObject.SetActive (false);
        }
    }

    private BaseUI getUI (BaseUI ui) {
        for (int i = 0; i < this.uiList.Count; i++) {
            if (this.uiList[i].Tag == ui) {
                return this.uiList[i];
            }
        }

        return null;
    }

    private void showUI (BaseUI ui, Action callBack = null) {
        BaseUI bUI = this.getUI (ui);
        if (ui != null) {
            ui.gameObject.SetActive (true);
            if (callBack != null) {
                callBack ();
            }

            bUI.onShow ();
        } else {
            this.openUI (ui, () => {
                if (callBack != null) {
                    callBack ();
                }
                BaseUI ui1 = this.getUI (ui);
                bUI.onShow ();
            });
        }
    }

    private void showUI (BaseUI ui, Action callBack = null, params object[] args) {
        BaseUI bUI = this.getUI (ui);
        if (ui != null) {
            ui.gameObject.SetActive (true);
            if (callBack != null) {
                callBack ();
            }

            bUI.onShow (args);
        } else {
            this.openUI (ui, () => {
                if (callBack != null) {
                    callBack ();
                }
                BaseUI ui1 = this.getUI (ui);
                bUI.onShow (args);
            });
        }
    }

    private void openUI (BaseUI ui, Action callBack) {
        string url = ui.getUrl ();

        GameObject prefab = Resources.Load<GameObject> (url);

        GameObject uiNode = GameObject.Instantiate (prefab);
        uiNode.transform.parent = this.uiRoot.transform;
        BaseUI bUI = uiNode.GetComponent<BaseUI> ();
        this.uiList.Add (bUI);
        if (callBack != null) {
            callBack ();
        }
    }
}