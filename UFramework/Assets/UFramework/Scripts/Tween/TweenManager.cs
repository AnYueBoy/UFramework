/*
 * @Author: l hy 
 * @Date: 2021-12-08 18:04:44 
 * @Description: Tween管理
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UFramework.Tween
{
    public class TweenManager : ITweenManager
    {
        private List<ITweener> curTweenerList = new List<ITweener>();
        private List<ITweener> curRemoveTweenerList = new List<ITweener>();

        private Dictionary<Type, List<ITweener>> tweenerPool = new Dictionary<Type, List<ITweener>>();

        public void LocalUpdate(float dt)
        {
            // 不受timescale=0 影响的dt
            float unscaleTime = Time.unscaledDeltaTime;
            var curActiveScene = SceneManager.GetActiveScene().name;
            for (int i = curTweenerList.Count - 1; i >= 0; i--)
            {
                var tweener = curTweenerList[i];
                if (!tweener.BindSceneName.Equals(curActiveScene))
                {
                    RemoveTweener(tweener);
                    continue;
                }

                if (tweener.TweenerState != TweenerState.Working)
                {
                    continue;
                }

                if (tweener.TimeScaleAffected())
                {
                    tweener.LocalUpdate(dt);
                }
                else
                {
                    tweener.LocalUpdate(unscaleTime);
                }
            }

            foreach (ITweener removeTweener in curRemoveTweenerList)
            {
                curTweenerList.Remove(removeTweener);
            }

            curRemoveTweenerList.Clear();
        }

        public T2 SpawnTweener<T1, T2>() where T2 : Tweener<T1>, new()
        {
            Type type = typeof(T2);
            T2 tweener;
            if (!tweenerPool.ContainsKey(type))
            {
                List<ITweener> tweenerList = new List<ITweener>();
                tweenerPool.Add(type, tweenerList);
                tweener = new T2();
            }
            else
            {
                List<ITweener> tweenerList = tweenerPool[type];
                if (tweenerList.Count <= 0)
                {
                    tweener = new T2();
                }
                else
                {
                    tweener = (T2)tweenerList[0];
                    tweenerList.Remove(tweener);
                }
            }

            tweener.Init(SceneManager.GetActiveScene().name, type);

            curTweenerList.Add(tweener);
            return tweener;
        }

        public void RemoveTweener(ITweener tweener)
        {
            curRemoveTweenerList.Add(tweener);
            Type poolType = tweener.TweenerType;
            List<ITweener> tweenerList = tweenerPool[poolType];
            tweenerList.Add(tweener);
            tweener.ResetTweener();
        }
    }
}