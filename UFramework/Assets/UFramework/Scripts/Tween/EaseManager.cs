/*
 * @Author: l hy 
 * @Date: 2021-12-10 18:19:54 
 * @Description: 缓动函数管理
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UFramework.Tween
{
    public class EaseManager
    {
        private static Dictionary<EaseType, Func<float, float, float>> easeDic =
            new Dictionary<EaseType, Func<float, float, float>>()
            {
                { EaseType.LINER, Liner }, { EaseType.InQuad, InQuad }, { EaseType.OutQuad, OutQuad },
                { EaseType.InCubic, InCubic },
                { EaseType.OutCubic, OutCubic }, { EaseType.InQuart, InQuart }, { EaseType.OutQuart, OutQuart },
                { EaseType.InOutSine, InOutSine },
            };

        private static float Liner(float time, float duration)
        {
            float ratioTime = time / duration;
            return ratioTime;
        }

        private static float InQuad(float time, float duration)
        {
            float ratioTime = time / duration;
            return ratioTime * ratioTime;
        }

        private static float OutQuad(float time, float duration)
        {
            float ratioTime = time / duration;
            return 1 - (1 - ratioTime) * (1 - ratioTime);
        }

        private static float OutCubic(float time, float duration)
        {
            float ratioTime = time / duration;
            float endValue = --ratioTime * ratioTime * ratioTime + 1.0f;
            return endValue;
        }

        private static float InCubic(float time, float duration)
        {
            float ratioTime = time / duration;
            return ratioTime * ratioTime * ratioTime;
        }

        private static float InQuart(float time, float duration)
        {
            float ratioTime = time / duration;
            return ratioTime * ratioTime * ratioTime * ratioTime;
        }

        private static float OutQuart(float time, float duration)
        {
            float ratioTime = time / duration;
            float endValue = 1 - Mathf.Pow(1 - ratioTime, 4);
            return endValue;
        }

        public static float InOutSine(float time, float duration)
        {
            float ratioTime = time / duration;
            float endValue = -(Mathf.Cos((float)Math.PI * ratioTime) - 1) / 2;
            return endValue;
        }

        public static float GetEaseFuncValue(EaseType ease, float time, float duration)
        {
            Func<float, float, float> easeFunc = easeDic[ease];
            return easeFunc.Invoke(time, duration);
        }
    }

    public enum EaseType
    {
        LINER,
        InQuad,
        OutQuad,
        InCubic,
        OutCubic,
        InQuart,
        OutQuart,
        InOutSine,
    }
}