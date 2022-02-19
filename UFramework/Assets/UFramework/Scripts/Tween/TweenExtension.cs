/*
 * @Author: l hy 
 * @Date: 2021-12-08 18:15:12 
 * @Description: TweenExtension
 */

using System.Collections.Generic;
using UFramework.Core;
using UFramework.Tween.Core;
using UnityEngine;
namespace UFramework.Tween {

    public static class TweenExtension {
        public static TweenerTransform<Vector3> PathTween (this Transform target, List<Vector3> pathList, float duration) {
            if (pathList.Count <= 1) {
                return MoveTween (target, pathList[0], duration);
            }

            TweenerTransform<Vector3> tweener = App.Make<ITweenManager> ().SpawnTweener<Vector3, TweenerTransform<Vector3>> ();
            TweenerCore<Vector3> tweenerCore = new TweenerCore<Vector3> (
                () => {
                    return target.transform.position;
                },
                (Vector3 value) => {
                    target.transform.position = value;
                },
                duration
            );

            pathList.Insert (0, target.position);

            float distance = 0;
            for (int i = 0; i < pathList.Count - 1; i++) {
                Vector3 prePos = pathList[i];
                Vector3 curPos = pathList[i + 1];
                distance += (curPos - prePos).magnitude;
            }

            tweenerCore.changeValue = Vector3.one * distance;

            tweener.SetTweenCore (tweenerCore);
            tweener.SetExtraData<List<Vector3>> (pathList);
            tweener.SetExecuteAction (tweener.PathTween);
            return tweener;
        }

        public static TweenerTransform<Vector3> MoveTween (this Transform target, Vector3 endPos, float duration) {
            TweenerTransform<Vector3> tweener = App.Make<ITweenManager> ().SpawnTweener<Vector3, TweenerTransform<Vector3>> ();

            TweenerCore<Vector3> tweenerCore = new TweenerCore<Vector3> (
                () => {
                    return target.transform.position;
                },
                (Vector3 value) => {
                    target.transform.position = value;
                },
                duration
            );

            tweenerCore.changeValue = endPos - tweenerCore.beginValue;

            tweener.SetTweenCore (tweenerCore);
            tweener.SetExecuteAction (tweener.MoveTween);
            return tweener;
        }
    }

}