/*
 * @Author: l hy 
 * @Date: 2021-12-08 18:04:44 
 * @Description: Tween管理
 */

namespace UFramework.Tween {
    using System.Collections.Generic;
    using System;

    public static class TweenManager {

        private static HashSet<ITweener> tweeners = new HashSet<ITweener> ();

        private static HashSet<ITweener> removeList = new HashSet<ITweener> ();

        private static Dictionary<Type, List<ITweener>> tweenerPool = new Dictionary<Type, List<ITweener>> ();
        public static void localUpdate (float dt) {
            foreach (ITweener tweener in tweeners) {
                tweener.localUpdate (dt);
            }

            foreach (ITweener removeTweener in removeList) {
                tweeners.Remove (removeTweener);
            }

            removeList.Clear ();
        }

        public static T2 spawnTweener<T1, T2> () where T2 : Tweener<T1>, new () {
            Type type = typeof (T2);
            T2 tweener = null;
            if (!tweenerPool.ContainsKey (type)) {
                List<ITweener> tweenerList = new List<ITweener> ();
                tweenerPool.Add (type, tweenerList);
                tweener = new T2 ();
            } else {
                List<ITweener> tweenerList = tweenerPool[type];
                if (tweenerList.Count <= 0) {
                    tweener = new T2 ();
                } else {
                    tweener = (T2) tweenerList[0];
                    tweenerList.Remove (tweener);
                }
            }

            tweener.init ((Tweener<T1> tweenerInstance) => {
                removeTween<T2> (tweenerInstance);
            });

            tweeners.Add (tweener);
            return tweener;
        }

        private static void removeTween<T> (ITweener tweener) {
            removeList.Add (tweener);
            Type poolType = typeof (T);
            List<ITweener> tweenerList = tweenerPool[poolType];
            tweenerList.Add (tweener);
        }

    }
}