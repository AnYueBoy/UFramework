/*
 * @Author: l hy 
 * @Date: 2021-12-10 18:19:54 
 * @Description: 缓动函数管理
 */

using System;
using System.Collections.Generic;
namespace UFramework.Tween {

    public class EaseManager {

        private static Dictionary<EaseType, Func<float, float, float>> easeDic =
            new Dictionary<EaseType, Func<float, float, float>> () { { EaseType.LINER, Liner }, { EaseType.InQuad, InQuad }, { EaseType.InCubic, InCubic }, { EaseType.OutCubic, OutCubic }
            };

        private static float Liner (float time, float duration) {
            float ratioTime = time / duration;
            return ratioTime;
        }

        private static float InQuad (float time, float duration) {
            float ratioTime = time / duration;
            return ratioTime * ratioTime;
        }

        private static float OutCubic (float time, float duration) {
            float ratioTime = time / duration;
            float endValue = --ratioTime * ratioTime * ratioTime + 1.0f;
            return endValue;
        }

        private static float InCubic (float time, float duration) {
            float ratioTime = time / duration;
            return ratioTime * ratioTime * ratioTime;
        }

        public static float GetEaseFuncValue (EaseType ease, float time, float duration) {
            Func<float, float, float> easeFunc = easeDic[ease];
            return easeFunc.Invoke (time, duration);
        }
    }

    public enum EaseType {
        LINER,
        InQuad,
        OutCubic,
        InCubic,
    }
}