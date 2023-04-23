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
        imageTrans.TweenerLocalMove(new Vector3(0, 100, 0), 2.0f).SetInitialValue(Vector3.zero)
            .SetEase(EaseType.LINER).SetLoop(-1, LoopType.YoYo);
    }

    // Update is called once per frame
    void Update()
    {
    }
}