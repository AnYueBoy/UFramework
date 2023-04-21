/*
 * @Author: l hy 
 * @Date: 2021-12-08 18:24:38 
 * @Description: Tweener
 */

using System;

namespace UFramework.Tween
{
    public class Tweener<T> : ITweener
    {
        protected TweenerCore<T> tweenerCore;
        protected float timer;

        private Action<float, TweenerCore<T>> executeHandler;
        private object extraData;
        private Action<Tweener<T>> tweenerOverCallback;

        public void Init(Action<Tweener<T>> tweenerOverCallback)
        {
            this.tweenerOverCallback = tweenerOverCallback;
        }

        public void SetExecuteAction(Action<float, TweenerCore<T>> actionHandler)
        {
            executeHandler = actionHandler;
        }

        public void LocalUpdate(float dt)
        {
            executeHandler?.Invoke(dt, tweenerCore);
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

        private void ResetExtraData()
        {
            extraData = null;
        }

        public Tweener<T> SetEase(EaseType easeType)
        {
            tweenerCore.easeTye = easeType;
            return this;
        }

        public Tweener<T> SetCompleted(Action callback)
        {
            tweenerCore.completedCallback = callback;
            return this;
        }

        private void TriggerCompleted()
        {
            tweenerCore.completedCallback?.Invoke();
            tweenerCore.completedCallback = null;
        }

        protected void TweenerCompleted()
        {
            executeHandler = null;
            timer = 0;
            ResetExtraData();
            TriggerCompleted();
            tweenerOverCallback?.Invoke(this);
        }
    }
}