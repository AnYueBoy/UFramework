/*
 * @Author: l hy 
 * @Date: 2021-02-23 21:39:35 
 * @Description: 模块管理
 * @Last Modified by: l hy
 * @Last Modified time: 2021-02-23 21:52:31
 */

namespace UFramework {
    using UFramework.Promise;
    using UnityEngine;
    public class ModuleManager : MonoBehaviour {
        private static ModuleManager _instance;

        public static ModuleManager instance {
            get {
                return _instance;
            }
        }

        #region mono模块

        #endregion

        #region 非mono模块
        public PromiseTimer promiseTimer = new PromiseTimer ();
        #endregion

        #region  程序生命周期函数 
        private void Awake () {
            _instance = this;
        }

        private void Start () {

        }

        private void Update () {
            float dt = Time.deltaTime;
            this.promiseTimer?.localUpdate (dt);
        }

        private void LateUpdate () {

        }

        private void OnGUI () {
        }

        private void OnDisable () {
        }
        #endregion 

    }
}