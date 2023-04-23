/*
 * @Author: l hy 
 * @Date: 2021-12-10 08:51:06 
 * @Description: 
 */

using System.Collections.Generic;
using UnityEngine;

namespace UFramework.Tween
{
    public class TweenerTransform<T> : Tweener<T>
    {
        public void ScaleTweener(float dt, TweenerCore<Vector3> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);
            Vector3 endValue = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter(endValue);
        }

        public void RotateTweener(float dt, TweenerCore<Vector3> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);
            Vector3 endValue = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter(endValue);
        }

        public void MoveTweener(float dt, TweenerCore<Vector3> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);
            Vector3 endValue = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter(endValue);
        }

        public void AnchoredMoveTweener(float dt, TweenerCore<Vector2> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);
            Vector2 endValue = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter(endValue);
        }

        public void SizeTweener(float dt, TweenerCore<Vector2> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);
            Vector3 endValue = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter(endValue);
        }

        public void MoveBezierSecondOrderTweener(float dt, TweenerCore<Vector3> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);

            // 控制点1
            var controlPoint1 = GetExtraData<Vector3>();

            // 起点位置
            Vector3 startPos = tweenerCore.beginValue;
            // 终点位置
            Vector3 endPos = tweenerCore.endValue;

            // 计算的值
            Vector3 endValue = (1 - ratioValue) * (1 - ratioValue) * startPos +
                               2 * ratioValue * (1 - ratioValue) * controlPoint1 + ratioValue * ratioValue * endPos;
            tweenerCore.setter(endValue);
        }

        public void MoveBezierThirdOrderTweener(float dt, TweenerCore<Vector3> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);

            var extraData = GetExtraData<List<Vector3>>();
            // 控制点1
            Vector3 controlPoint1 = extraData[0];
            // 控制点2
            Vector3 controlPoint2 = extraData[1];

            // 起点位置
            Vector3 startPos = tweenerCore.beginValue;
            // 终点位置
            Vector3 endPos = tweenerCore.endValue;

            // 计算的值
            Vector3 endValue = (1 - ratioValue) * (1 - ratioValue) * (1 - ratioValue) * startPos +
                               3 * ratioValue * (1 - ratioValue) * (1 - ratioValue) * controlPoint1 +
                               3 * ratioValue * ratioValue * (1 - ratioValue) * controlPoint2 +
                               ratioValue * ratioValue * ratioValue * endPos;

            tweenerCore.setter(endValue);
        }
    }
}