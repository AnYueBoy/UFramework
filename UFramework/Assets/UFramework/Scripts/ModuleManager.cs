/*
 * @Author: l hy 
 * @Date: 2021-02-23 21:39:35 
 * @Description: 模块管理
 * @Last Modified by: l hy
 * @Last Modified time: 2021-02-23 21:52:31
 */

namespace UFramework {
    using UFramework.Promise;
    using UFramework.Const;
    using UFramework.Develop;
    using UnityEngine;
    public class ModuleManager : MonoBehaviour {
        public AppMode appMode = AppMode.Developing;

        private static ModuleManager _instance;

        public static ModuleManager instance {
            get {
                return _instance;
            }
        }

        #region mono模块

        #endregion

        #region 非mono模块
        private GUIConsole guiConsole = new GUIConsole ();
        public PromiseTimer promiseTimer = new PromiseTimer ();
        #endregion

        #region  程序生命周期函数 
        private void Awake () {
            _instance = this;
            if (this.appMode != AppMode.Release) {
                this.guiConsole.init ();
            }
        }

        private void Start () {

        }

        private void Update () {
            float dt = Time.deltaTime;
            this.guiConsole?.localUpdate (dt);
            this.promiseTimer?.localUpdate (dt);
        }

        private void LateUpdate () {

        }

        private void OnGUI () {
            this.guiConsole?.drawGUI ();
        }

        private void OnDisable () {
            this.guiConsole?.quit ();
        }
        #endregion 

    }
}