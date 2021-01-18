/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:56:04 
 * @Description: 资源访问的统一对外接口
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-18 22:24:06
 */
namespace UFramework.GameCommon {

    using System.Collections.Generic;
    using UnityEngine;

    public class AssetsManager {

        private Dictionary<string, PackAsset> assetPool = new Dictionary<string, PackAsset> ();

        /// <summary>
        /// 获取指定资源
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T getAssetByUrl<T> (string assetUrl) where T : Object {
            Object targetAsset = null;
            if (assetPool.ContainsKey (assetUrl)) {
                targetAsset = assetPool[assetUrl].targetAsset;
                return targetAsset as T;
            }

            targetAsset = Resources.Load<T> (assetUrl);
            PackAsset packageAsset = new PackAsset (targetAsset);
            assetPool.Add (assetUrl, packageAsset);
            return targetAsset as T;
        }

        /// <summary>
        /// 尝试释放资源并返回释放结果
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <returns></returns>
        public bool tryReleaseAsset (string assetUrl) {
            if (!this.assetPool.ContainsKey (assetUrl)) {
                Debug.LogWarning ("can not release not exist asset");
                return false;
            }

            PackAsset packAsset = this.assetPool[assetUrl];
            bool releaseResult = packAsset.releaseAsset ();
            return releaseResult;
        }
    }
}