using System;

namespace UFramework.Tween
{
    public interface ITweener
    {
        /// <summary>
        /// 绑定的场景，用于场景卸载时，Tweener的回收
        /// </summary>
        string BindSceneName { get; set; }

        /// <summary>
        /// Tweener类型
        /// </summary>
        Type TweenerType { get; set; }

        TweenerState TweenerState { get; }

        void LocalUpdate(float dt);

        /// <summary>
        /// 设置缓动曲线
        /// </summary>
        ITweener SetEase(EaseType easeType);

        /// <summary>
        /// 设置循环
        /// </summary>
        ITweener SetLoop(int count = 0, LoopType loopType = LoopType.ReStart);

        /// <summary>
        /// 设置完成回调
        /// </summary>
        ITweener OnCompleted(Action callback);

        /// <summary>
        /// 完成回调
        /// </summary>
        event Action CompletedEvent;

        /// <summary>
        /// 更新设置值与获取值
        /// </summary>
        void UpdateGetterValue();

        /// <summary>
        /// 是否受时间放缩参数影响
        /// </summary>
        bool TimeScaleAffected();

        /// <summary>
        /// 是否受时间放缩因子影响
        /// </summary>
        ITweener SetTimeScaleAffected(bool timeScaleAffected);

        void Pause();

        void Resume();

        void TweenerKill();

        void ResetTweener();
    }

    interface ITweener<T> : ITweener
    {
        /// <summary>
        /// 设置初始值
        /// </summary>
        ITweener SetInitialValue(T value);

        /// <summary>
        /// 设置更新函数
        /// </summary>
        ITweener OnUpdate(Action<T> updateCallback);
    }
}