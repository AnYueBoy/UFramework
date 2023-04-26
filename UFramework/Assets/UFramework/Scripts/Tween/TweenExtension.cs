/*
 * @Author: l hy 
 * @Date: 2021-12-08 18:15:12 
 * @Description: TweenExtension
 */

using System;
using System.Collections.Generic;
using UFramework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UFramework.Tween
{
    public static class TweenerExtension
    {
        #region Transform

        public static TweenerTransform<Vector3> TweenerMove(this Transform target, Vector3 endPos, float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector3, TweenerTransform<Vector3>>();
            var tweenerCore = new TweenerCore<Vector3>(
                () => target.position,
                value => target.position = value,
                endPos,
                duration
            );

            // 设置重计算过渡值的回调
            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };

            tweener.calculateValueCallback?.Invoke();

            // 设置TweenerCore
            tweener.SetTweenCore(tweenerCore);
            // 设置执行函数
            tweener.SetExecuteAction(tweener.MoveTweener);

            return tweener;
        }

        public static TweenerTransform<Vector2> TweenerAnchoredPositionMove(this RectTransform target, Vector2 endPos,
            float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector2, TweenerTransform<Vector2>>();
            var tweenerCore = new TweenerCore<Vector2>(
                () => target.anchoredPosition,
                value => target.anchoredPosition = value,
                endPos,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();

            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.AnchoredMoveTweener);
            return tweener;
        }

        public static TweenerTransform<Vector2> TweenerAnchoredYMove(this RectTransform target, float endY,
            float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector2, TweenerTransform<Vector2>>();
            var endPos = target.anchoredPosition;
            endPos.y = endY;
            var tweenerCore = new TweenerCore<Vector2>(
                () => target.anchoredPosition,
                value => target.anchoredPosition = value,
                endPos,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();

            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.AnchoredMoveTweener);
            return tweener;
        }

        public static TweenerTransform<Vector3> TweenerLocalMove(this Transform target, Vector3 endPos, float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector3, TweenerTransform<Vector3>>();
            var tweenerCore = new TweenerCore<Vector3>(
                () => target.localPosition,
                value => target.localPosition = value,
                endPos,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.MoveTweener);
            return tweener;
        }

        public static TweenerTransform<Vector3> TweenerLocalMoveY(this Transform target, float endY, float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector3, TweenerTransform<Vector3>>();
            Func<Vector3> tweenerEndValueCallback = () =>
            {
                var endPos = target.localPosition;
                endPos.y = endY;
                return endPos;
            };

            var tweenerCore = new TweenerCore<Vector3>(
                () => target.localPosition,
                value => target.localPosition = value,
                tweenerEndValueCallback(),
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.endValue = tweenerEndValueCallback();
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();

            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.MoveTweener);
            return tweener;
        }

        public static TweenerTransform<Vector2> TweenerSize(this Transform target, Vector2 endSize, float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector2, TweenerTransform<Vector2>>();
            var tweenerCore = new TweenerCore<Vector2>(
                () => (target as RectTransform).sizeDelta,
                value => (target as RectTransform).sizeDelta = value,
                endSize,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();

            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.SizeTweener);
            return tweener;
        }

        /// <summary>
        /// 使用二阶贝塞尔曲线移动
        /// </summary>
        /// <param name="endPos">终点位置</param>
        /// <param name="duration">动画时间</param>
        /// <param name="controlPoint1">控制点1</param>
        public static TweenerTransform<Vector3> TweenerLocalMoveBySecondOrderBezier(this Transform target,
            Vector3 endPos,
            float duration, Vector3 controlPoint1)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector3, TweenerTransform<Vector3>>();
            var tweenerCore = new TweenerCore<Vector3>(
                () => target.localPosition,
                value => target.localPosition = value,
                endPos,
                duration
            );

            // 设置控制点的额外数据
            tweener.SetExtraData(controlPoint1);

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.MoveBezierSecondOrderTweener);
            return tweener;
        }

        /// <summary>
        /// 使用三阶贝塞尔曲线进行移动
        /// </summary>
        /// <param name="endPos">终点位置</param>
        /// <param name="duration">动画时间</param>
        /// <param name="controlPoint1">控制点1</param>
        /// <param name="controlPoint2">控制点2</param>
        public static TweenerTransform<Vector3> TweenerLocalMoveByThirdOrderBezier(this Transform target,
            Vector3 endPos, float duration, Vector3 controlPoint1, Vector3 controlPoint2)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector3, TweenerTransform<Vector3>>();
            var tweenerCore = new TweenerCore<Vector3>(
                () => target.localPosition,
                value => target.localPosition = value,
                endPos,
                duration
            );

            // 设置控制点的额外数据
            List<Vector3> extraData = new List<Vector3>() { controlPoint1, controlPoint2 };
            tweener.SetExtraData(extraData);

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.MoveBezierThirdOrderTweener);
            return tweener;
        }

        public static TweenerTransform<Vector3> TweenerScale(this Transform target, Vector3 endScale, float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector3, TweenerTransform<Vector3>>();
            var tweenerCore = new TweenerCore<Vector3>(
                () => target.localScale,
                value => target.localScale = value,
                endScale,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.ScaleTweener);
            return tweener;
        }


        public static TweenerTransform<Vector3> TweenerRotate(this Transform target, Vector3 endRotate, float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector3, TweenerTransform<Vector3>>();
            var tweenerCore = new TweenerCore<Vector3>(
                () =>
                {
                    var angle = target.localEulerAngles;
                    if (angle.z > 180)
                    {
                        // TODO:
                    }

                    return angle;
                },
                value => target.localEulerAngles = value,
                endRotate,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.RotateTweener);
            return tweener;
        }

        #endregion

        #region CanvasGroup

        public static TweenerCanvasGroup<float> TweenerFade(this CanvasGroup target, float endAlpha, float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<float, TweenerCanvasGroup<float>>();
            var tweenerCore = new TweenerCore<float>(
                () => target.alpha,
                value => target.alpha = value,
                endAlpha,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.AlphaTweener);
            return tweener;
        }

        #endregion

        #region Image

        public static TweenerImage<float> TweenerFillAmount(this Image target, float endValue, float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<float, TweenerImage<float>>();
            var tweenerCore = new TweenerCore<float>(
                () => target.fillAmount,
                value => target.fillAmount = value,
                endValue,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.FillAmountTweener);
            return tweener;
        }

        #endregion

        #region Value

        public static TweenerValue<float> TweenerValue(Func<float> getter, Action<float> setter, float endValue,
            float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<float, TweenerValue<float>>();
            var tweenerCore = new TweenerCore<float>(
                () => getter(),
                value => setter(value),
                endValue,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.ValueTween);
            return tweener;
        }

        public static TweenerValue<Vector3> TweenerValue(Func<Vector3> getter, Action<Vector3> setter, Vector3 endValue,
            float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector3, TweenerValue<Vector3>>();
            var tweenerCore = new TweenerCore<Vector3>(
                () => getter(),
                value => setter(value),
                endValue,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.ValueTween);
            return tweener;
        }

        public static TweenerValue<Vector2> TweenerValue(Func<Vector2> getter, Action<Vector2> setter, Vector2 endValue,
            float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<Vector2, TweenerValue<Vector2>>();
            var tweenerCore = new TweenerCore<Vector2>(
                () => getter(),
                value => setter(value),
                endValue,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.ValueTween);
            return tweener;
        }

        #endregion

        #region Timer

        public static TweenerTimer<float> TweenerTimer(float duration)
        {
            var tweener = App.Make<ITweenManager>().SpawnTweener<float, TweenerTimer<float>>();
            float startTime = 0;
            float endTime = duration;
            var tweenerCore = new TweenerCore<float>(
                () => startTime,
                value => endTime = value,
                endTime,
                duration
            );

            tweener.calculateValueCallback = () =>
            {
                tweenerCore.changeValue = tweenerCore.endValue - tweenerCore.beginValue;
            };
            tweener.calculateValueCallback?.Invoke();
            tweener.SetTweenCore(tweenerCore);
            tweener.SetExecuteAction(tweener.TimerTween);
            return tweener;
        }

        #endregion
    }
}