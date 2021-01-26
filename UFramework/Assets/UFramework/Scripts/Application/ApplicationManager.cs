/*
 * @Author: l hy 
 * @Date: 2021-01-25 15:34:12 
 * @Description: 程序入口
 */
namespace UFrameWork.Application {
    using System;
    using UFrameWork.LogSystem;
    using UnityEngine;

    public delegate void applicationCallback ();
    public class ApplicationManager : MonoBehaviour {

        [Header ("当前程序模式")]
        public AppMode appMode = AppMode.Developing;

        private void Awake () {
            appLaunch ();
        }

        private void gameStart () {

        }

        #region 程序生命周期
        public static applicationCallback applicationQuit = null;
        public static applicationCallback applicationUpdate = null;
        public static applicationCallback applicationOnGUI = null;

        private void onApplicationQuit () {
            if (applicationQuit != null) {
                try {
                    applicationQuit ();
                } catch (Exception e) {
                    Debug.LogError (e.ToString ());
                }
            }
        }

        private void Update () {
            if (applicationUpdate != null) {
                applicationUpdate ();
            }
        }

        private void OnGUI () {
            if (applicationOnGUI != null) {
                applicationOnGUI ();
            }
        }
        #endregion

        #region 程序启动细节

        private void setResourceLoadType () {
            // TODO: 资源加载方式
        }

        private void appLaunch () {
            setResourceLoadType ();

            // 启动日志
            LogManager.init ();

            if (appMode != AppMode.Release) {
                //TODO: 图形控制面板初始化
            }

            // 热更
            hotUpdate ();
        }

        public void hotUpdate () {

        }

        public void hoyUpdateCompleted () {
            // TODO: bundle配置初始化
            // TODO: uimanger初始化
            gameStart ();

        }
        #endregion
    }
}