/*
 * @Author: l hy 
 * @Date: 2020-03-07 17:13:07 
 * @Description: 界面管理器 
 * @Last Modified by: l hy
 * @Last Modified time: 2021-05-05 10:30:58
 */
namespace UFramework.GameCommon {

    using System.Collections.Generic;
    using System;
    using UFramework.Const;
    using UnityEngine;

    public class UIManager {

        private Dictionary<string, BaseUI> uiDic = new Dictionary<string, BaseUI> ();

        private BaseUI currentBoard = null;

        private GameObject uiRoot;

        public void init (GameObject uiRoot) {
            this.uiRoot = uiRoot;
            this.currentBoard = null;
        }

        public void showBoard (string uiName, params object[] args) {
            BaseUI targerUI = this.getUI (uiName);
            if (currentBoard != null && this.currentBoard == targerUI) {
                return;
            }

            this.showUI (uiName, args);
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

        private void showUI (string uiName, params object[] args) {
            BaseUI targetUI = this.getUI (uiName);
            if (targetUI != null) {
                targetUI.gameObject.SetActive (true);
            } else {
                string url = UrlString.uiUrl + uiName;
                GameObject prefab = Resources.Load<GameObject> (url);

                GameObject uiNode = GameObject.Instantiate (prefab);
                uiNode.transform.SetParent (this.uiRoot.transform);
                uiNode.transform.localPosition = Vector3.zero;
                targetUI = uiNode.GetComponent<BaseUI> ();
                this.uiDic.Add (uiName, targetUI);
                uiNode.SetActive (true);
            }

            if (this.currentBoard != null) {
                this.currentBoard.gameObject.SetActive (false);
            }

            this.currentBoard = targetUI;

            this.currentBoard.onShow (args);
        }
    }
}