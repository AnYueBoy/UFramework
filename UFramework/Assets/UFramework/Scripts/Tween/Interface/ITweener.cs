using System;

namespace UFramework.Tween
{
    public interface ITweener
    {
        /// <summary>
        /// 绑定的场景，用于场景卸载时，Tweener的回收
        /// </summary>
        string BindSceneName { get; set; }

        Type TweenerType { get; set; }

        void LocalUpdate(float dt);

        /// <summary>
        /// 设置初始值
        /// </summary>
        ITweener SetEase(EaseType easeType);

        ITweener SetLoop(int count = 0, LoopType loopType = LoopType.YoYo);

        ITweener OnCompleted(Action callback);

        /// <summary>
        /// 是否受时间放缩参数影响
        /// </summary>
        bool TimeScaleAffected();

        /// <summary>
        /// 是否受时间放缩因子影响
        /// </summary>
        ITweener SetTimeScaleAffected(bool timeScaleAffected);

        void TweenerKill();

        void ResetTweener();
    }

    interface ITweener<T> : ITweener
    {
        ITweener SetInitialValue(T value);
    }
}