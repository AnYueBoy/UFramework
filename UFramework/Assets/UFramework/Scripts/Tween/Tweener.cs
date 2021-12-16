/*
 * @Author: l hy 
 * @Date: 2021-12-08 18:24:38 
 * @Description: Tweener
 */
namespace UFramework.Tween {
    using System;
    using UFramework.Tween.Core;
    public class Tweener<T> : ITweener {

        protected TweenerCore<T> tweenerCore;
        protected float timer;

        private Action<float, TweenerCore<T>> executeHandler;
        private object extraData;
        private Action<Tweener<T>> tweenerOverCallback;

        public void init (Action<Tweener<T>> tweenerOverCallback) {
            this.tweenerOverCallback = tweenerOverCallback;
        }

        public void setExecuteAction (Action<float, TweenerCore<T>> actionHandler) {
            this.executeHandler = actionHandler;
        }

        public void localUpdate (float dt) {
            executeHandler?.Invoke (dt, this.tweenerCore);
        }

        public void setTweenCore (TweenerCore<T> tweenCore) {
            this.tweenerCore = tweenCore;
        }

        public void setExtraData<T1> (T1 extraData) {
            this.extraData = extraData;
        }

        protected T1 getExtraData<T1> () {
            return (T1) this.extraData;
        }

        private void resetExtraData () {
            this.extraData = null;
        }

        public Tweener<T> setEase (EaseType easeType) {
            this.tweenerCore.easeTye = easeType;
            return this;
        }

        public Tweener<T> setCompleted (Action callback) {
            this.tweenerCore.completedCallback = callback;
            return this;
        }

        private void triggerCompleted () {
            this.tweenerCore.completedCallback?.Invoke ();
            this.tweenerCore.completedCallback = null;
        }

        protected void tweenerCompleted () {
            this.executeHandler = null;
            this.timer = 0;
            this.resetExtraData ();
            this.triggerCompleted ();
            this.tweenerOverCallback?.Invoke (this);
        }
    }

}

namespace UFramework.Tween.Core {
    using System;

    public delegate T TweenGetter<out T> ();

    public delegate void TweenSetter<in T> (T newValue);

    public class TweenerCore<T> {
        public TweenGetter<T> getter;

        public TweenSetter<T> setter;

        public EaseType easeTye;

        public T beginValue;

        public T changeValue;

        public float duration;

        public Action completedCallback;

        public TweenerCore (TweenGetter<T> getter, TweenSetter<T> setter, float duration) {
            this.getter = getter;
            this.setter = setter;
            this.duration = duration;
            this.beginValue = getter ();
            easeTye = EaseType.LINER;
        }
    }
}