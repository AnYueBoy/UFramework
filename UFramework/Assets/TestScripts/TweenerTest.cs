using System.Collections;
using System.Collections.Generic;
using UFramework.Core;
using UFramework.Tween;
using UnityEngine;

public class TweenerTest : MonoBehaviour
{
    [SerializeField] private Transform imageTrans;

    void Start()
    {
        // TweenValueTest();
        TweeneSequenceTest();
    }

    private void TweenValueTest()
    {
        float tweenValue = 10;
        TweenerExtension.TweenerValue(() => tweenValue, value => tweenValue = value, 0, 2.0f)
            .OnUpdate(value => { Debug.Log($"curValue: {value}"); })
            .OnCompleted(() => { Debug.Log($"endValue: {tweenValue}"); });
    }

    private void TweeneSequenceTest()
    {
        var s = new TweenerSequence();
        s.Append(imageTrans.TweenerLocalMove(Vector3.right * 100, 2.0f).SetInitialValue(Vector3.zero))
            .AppendInterval(1.5f)
            .Append(imageTrans.TweenerLocalMoveY(100, 2.0f)).AppendCallback(() => { Debug.Log("队列结束"); });
    }
}