/*
 * @Author: l hy 
 * @Date: 2021-12-10 08:51:06 
 * @Description: 
 */

using System.Collections.Generic;
using UFramework.Tween.Core;
using UnityEngine;
namespace UFramework.Tween {

    public class TweenerTransform<T> : Tweener<T> {

        public void PathTween (float dt, TweenerCore<Vector3> tweenerCore) {
            this.timer += dt;
            if (this.timer > tweenerCore.duration) {
                this.TweenerCompleted ();
                return;
            }

            float time = Mathf.Min (tweenerCore.duration, this.timer);
            float ratioValue = EaseManager.GetEaseFuncValue (tweenerCore.easeTye, time, tweenerCore.duration);
            float curMoveDistance = tweenerCore.changeValue.x * ratioValue;

            List<Vector3> pathList = this.GetExtraData<List<Vector3>> ();
            float cumulativeDis = 0;
            for (int i = 0; i < pathList.Count - 1; i++) {
                Vector3 prePos = pathList[i];
                Vector3 curPos = pathList[i + 1];
                float curPointDistance = (curPos - prePos).magnitude;
                cumulativeDis += curPointDistance;

                if (curMoveDistance < cumulativeDis) {
                    Vector3 dir = curPos - prePos;
                    dir = dir.normalized;
                    float preStageDistance = cumulativeDis - curPointDistance;
                    Vector3 endPos = prePos + (curMoveDistance - preStageDistance) * dir;
                    tweenerCore.setter (endPos);
                    break;
                }
            }
        }

        public void MoveTween (float dt, TweenerCore<Vector3> tweenerCore) {
            this.timer += dt;
            if (this.timer > tweenerCore.duration) {
                this.TweenerCompleted ();
                return;
            }

            float time = Mathf.Min (tweenerCore.duration, this.timer);
            float ratioValue = EaseManager.GetEaseFuncValue (tweenerCore.easeTye, time, tweenerCore.duration);

            Vector3 endPos = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter (endPos);
        }

    }
}