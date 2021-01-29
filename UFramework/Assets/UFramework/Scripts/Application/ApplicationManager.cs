/*
 * @Author: l hy 
 * @Date: 2021-01-25 15:34:12 
 * @Description: 程序入口
 */
namespace UFrameWork.Application {
    using UFrameWork.Develop;
    using UnityEngine;

    public class ApplicationManager : MonoBehaviour {

        [Header ("当前程序模式")]
        public AppMode appMode = AppMode.Developing;

        private GUIConsole guiConsole = new GUIConsole ();

        #region  程序生命周期函数
        private void Awake () {
            appLaunch ();
        }

        private void gameStart () {
            // 游戏逻辑初始化
        }

        private void Update () {
            if (guiConsole != null) {
                guiConsole.localUpdate ();
            }
        }

        private void OnGUI () {
            if (guiConsole != null) {
                guiConsole.drawGUI ();
            }
        }

        private void onApplicationQuit () {
            // 程序退出逻辑 
            this.guiConsole.quit ();
        }
        #endregion

        #region 程序启动细节

        private void appLaunch () {
            setResourceLoadType ();

            if (appMode != AppMode.Release) {
                // 图形控制面板初始化
                guiConsole.init ();
            }

            // 热更
            hotUpdate ();
        }

        private void setResourceLoadType () {
            // TODO: 资源加载方式
        }

        public void hotUpdate () {

        }

        public void hotUpdateCompleted () {
            // TODO: bundle配置初始化
            // TODO: uimanger初始化
            gameStart ();

        }
        #endregion
    }
}