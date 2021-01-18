/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:30:23 
 * @Description: Mono工具类
 */
namespace UFramework.FrameUtil {

    using System.Collections;
    using System;
    using UnityEngine;

    public class MonoUtil : MonoBehaviour {

        public void delay (float time, Action onFinished) {
            StartCoroutine (delayCoroutine (time, onFinished));
        }

        private IEnumerator delayCoroutine (float time, Action finished) {
            yield return new WaitForSeconds (time);
            finished ();
        }

        [Header ("路径点")]
        public Vector2[] pathList = null;
        private void OnDrawGizmos () {
            if (this.pathList == null || this.pathList.Length <= 0) {
                return;
            }

            Color orginColor = Gizmos.color;
            Gizmos.color = Color.red;
            for (int i = 0; i < this.pathList.Length; i++) {
                Vector2 point = this.pathList[i];
                Gizmos.DrawSphere (point, 0.2f);
            }

            Gizmos.color = Color.yellow;
            for (int i = 0; i < this.pathList.Length - 1; i++) {
                Gizmos.DrawLine (this.pathList[i], this.pathList[i + 1]);
            }

            Gizmos.color = orginColor;
        }
    }
}