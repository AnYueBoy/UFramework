using System;

namespace UFramework.Tween
{
    public delegate T TweenGetter<out T>();

    public delegate void TweenSetter<in T>(T newValue);

    public class TweenerCore<T>
    {
        public TweenGetter<T> getter;

        public TweenSetter<T> setter;

        public EaseType easeTye;

        public T beginValue;

        public T changeValue;

        public float duration;

        public Action completedCallback;

        public TweenerCore(TweenGetter<T> getter, TweenSetter<T> setter, float duration)
        {
            this.getter = getter;
            this.setter = setter;
            this.duration = duration;
            beginValue = getter();
            easeTye = EaseType.LINER;
        }
    }
}