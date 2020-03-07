/*
 * @Author: l hy 
 * @Date: 2020-03-07 17:13:07 
 * @Description: 界面管理器 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-03-07 17:18:01
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private static UIManager instance = null;

    private List<BaseUI> uiList = new List<BaseUI> ();

    private BaseUI currentBoard = null;

    private BaseUI currentDialog = null;

    [Header ("uiRoot")]
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
}