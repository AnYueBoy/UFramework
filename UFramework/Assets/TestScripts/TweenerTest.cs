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
        TweenValueTest();
    }

    private void TweenValueTest()
    {
        float tweenValue = 10;
        TweenerExtension.TweenerValue(() => tweenValue, value => tweenValue = value, 0, 2.0f)
            .OnUpdate(value => { Debug.Log($"curValue: {value}"); })
            .OnCompleted(() => { Debug.Log($"endValue: {tweenValue}"); });
    }
}