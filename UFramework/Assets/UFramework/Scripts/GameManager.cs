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
    public class GameManager : MonoBehaviour {

        public PromiseTimer promiseTimer = new PromiseTimer ();

        private void Update () {
            float dt = Time.deltaTime;
            this.promiseTimer?.localUpdate (dt);
        }
    }
}