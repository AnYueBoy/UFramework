using System;

namespace UFramework
{
    public delegate T TweenGetter<out T>();

    public delegate void TweenSetter<in T>(T newValue);

    public class TweenerCore<T>
    {
        public TweenGetter<T> getter;

        public TweenSetter<T> setter;

        public EaseType easeType;

        public T beginValue;

        public T changeValue;

        public T endValue;

        public readonly float duration;

        /// <summary>
        /// 当前执行的次数
        /// </summary>
        public int curExecuteCount;

        /// <summary>
        /// 需要执行的次数
        /// </summary>
        public int needExecuteCount;

        /// <summary>
        /// 循环类型
        /// </summary>
        public LoopType loopType;

        public TweenerCore(TweenGetter<T> getter, TweenSetter<T> setter, T endValue, float duration)
        {
            this.getter = getter;
            this.setter = setter;
            this.duration = duration;
            beginValue = getter();
            this.endValue = endValue;
            easeType = EaseType.LINER;
            curExecuteCount = needExecuteCount = 1;
            loopType = LoopType.ReStart;
        }
    }
}