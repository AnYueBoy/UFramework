/*
 * @Author: l hy 
 * @Date: 2021-02-23 21:39:35 
 * @Description: 模块管理
 * @Last Modified by: l hy
 * @Last Modified time: 2021-02-23 21:52:31
 */

namespace UFrameWork {
    using UFrameWork.Const;
    using UFrameWork.Develop;
    using UnityEngine;
    public class ModuleManager : MonoBehaviour {
        public AppMode appMode = AppMode.Developing;

        #region mono模块

        #endregion

        #region 非mono模块
        private GUIConsole guiConsole = new GUIConsole ();
        #endregion

        #region  程序生命周期函数 
        private void Awake () {
            if (this.appMode != AppMode.Release) {
                this.guiConsole.init ();
            }
        }

        private void Start () {

        }

        private void Update () {
            float dt = Time.deltaTime;
            if (this.guiConsole != null) {
                this.guiConsole.localUpdate (dt);
            }

        }

        private void LateUpdate () {

        }

        private void OnGUI () {
            if (guiConsole != null) {
                this.guiConsole.drawGUI ();
            }
        }

        private void OnDisable () {
            if (this.guiConsole != null) {
                this.guiConsole.quit ();
            }
        }
        #endregion 

    }
}