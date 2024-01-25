using System.Collections;
using System.Collections.Generic;
using UFramework;
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
            .Append(imageTrans.TweenerLocalMoveY(100, 2.0f))
            .Append(imageTrans.TweenerMove(Vector3.zero, 2.0f))
            .Append(imageTrans.TweenerMove(Vector3.zero, 2.0f))
            .AppendCallback(() => { Debug.Log("队列结束"); });
    }

    private void TweeneSequenceTest1()
    {
        var s = TweenerExtension.Sequence();
        s.Append(imageTrans.TweenerLocalMove(Vector3.right * 100, 2.0f))
            .Append(imageTrans.TweenerLocalMoveY(100, 2.0f))
            .Append(imageTrans.TweenerMove(Vector3.zero, 2.0f))
            .Append(imageTrans.TweenerMove(Vector3.zero, 2.0f));
    }

    private void TweeneCompleted()
    {
        imageTrans.TweenerLocalMove(Vector3.right * 100, 2.0f).OnCompleted(() =>
        {
            imageTrans.TweenerLocalMoveY(100, 2.0f).OnCompleted(() =>
            {
                imageTrans.TweenerMove(Vector3.zero, 2.0f).OnCompleted(() =>
                {
                    imageTrans.TweenerMove(Vector3.zero, 2.0f);
                });
            });
        });
    }
}