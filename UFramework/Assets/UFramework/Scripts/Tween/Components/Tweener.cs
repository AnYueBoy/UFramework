/*
 * @Author: l hy 
 * @Date: 2021-12-08 18:24:38 
 * @Description: Tweener
 */

using System;
using UFramework.Core;

namespace UFramework.Tween
{
    public class Tweener<T> : ITweener
    {
        /// <summary>
        /// tweener 执行的核心数据
        /// </summary>
        protected TweenerCore<T> tweenerCore;

        protected float timer;

        private Action<float, TweenerCore<T>> executeHandler;

        private object extraData;

        public Type TweenerType { get; set; }

        public string BindSceneName { get; set; }

        /// <summary>
        /// 重计算数据
        /// </summary>
        public Action calculateValueCallback;

        /// <summary>
        /// 是否受时间放缩因子影响
        /// </summary>
        private bool timeScaleAffected;

        public void Init(string bindSceneName,Type tweenerType)
        {
            timeScaleAffected = true;
            BindSceneName = bindSceneName;
            TweenerType = tweenerType;
        }

        public ITweener SetInitialValue(T value)
        {
            tweenerCore.beginValue = value;
            tweenerCore.setter(value);
            calculateValueCallback?.Invoke();
            return this;
        }

        public void SetExecuteAction(Action<float, TweenerCore<T>> actionHandler)
        {
            executeHandler = actionHandler;
        }

        public void LocalUpdate(float dt)
        {
            executeHandler?.Invoke(dt, tweenerCore);
        }

        public bool TimeScaleAffected()
        {
            return timeScaleAffected;
        }

        public ITweener SetTimeScaleAffected(bool timeScaleAffected)
        {
            this.timeScaleAffected = timeScaleAffected;
            return this;
        }

        public void SetTweenCore(TweenerCore<T> tweenCore)
        {
            tweenerCore = tweenCore;
        }

        public void SetExtraData<T1>(T1 extraData)
        {
            this.extraData = extraData;
        }

        protected T1 GetExtraData<T1>()
        {
            return (T1)extraData;
        }

        public ITweener SetEase(EaseType easeType)
        {
            tweenerCore.easeType = easeType;
            return this;
        }

        public ITweener SetLoop(int count = 0, LoopType loopType = LoopType.ReStart)
        {
            tweenerCore.needExecuteCount = count;
            tweenerCore.loopType = loopType;
            return this;
        }

        private void InvertBeginEndValue()
        {
            var tempValue = tweenerCore.beginValue;
            tweenerCore.beginValue = tweenerCore.endValue;
            tweenerCore.endValue = tempValue;
            calculateValueCallback?.Invoke();
        }

        public ITweener OnCompleted(Action callback)
        {
            tweenerCore.completedCallback = callback;
            return this;
        }

        protected void TriggerCompleted()
        {
            tweenerCore.setter?.Invoke(tweenerCore.endValue);
            // 重置时间计时器
            timer = 0;
            // 触发自定义完成回调
            tweenerCore.completedCallback?.Invoke();
            tweenerCore.curExecuteCount++;
            // 如果循环类型为YoYo则反转开始结束值
            if (tweenerCore.loopType == LoopType.YoYo)
            {
                // 反转开始结束值
                InvertBeginEndValue();
            }

            // 无限循环
            if (tweenerCore.needExecuteCount == -1)
            {
                return;
            }

            // 未达到循环次数
            if (tweenerCore.curExecuteCount < tweenerCore.needExecuteCount)
            {
                return;
            }

            // 自定义完成回调置空
            tweenerCore.completedCallback = null;

            // 执行函数置空
            executeHandler = null;

            // 回收
            App.Make<ITweenManager>().RemoveTweener(this);
        }

        public void TweenerKill()
        {
            ResetTweener();
            App.Make<ITweenManager>().RemoveTweener(this);
        }

        public void ResetTweener()
        {
            tweenerCore = null;
            timer = 0;
            executeHandler = null;
            extraData = null;
            calculateValueCallback = null;
        }
    }
}