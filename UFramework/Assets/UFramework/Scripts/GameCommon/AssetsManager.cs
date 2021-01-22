using System;
/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:56:04 
 * @Description: 资源访问的统一对外接口
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-21 22:05:33
 */
namespace UFramework.GameCommon {

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class AssetsManager {

        private Dictionary<string, PackAsset> assetPool = new Dictionary<string, PackAsset> ();

        /// <summary>
        /// 获取指定资源
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T getAssetByUrlSync<T> (string assetUrl) where T : Object {
            T targetAsset = null;
            PackAsset packAsset = null;
            if (assetPool.ContainsKey (assetUrl)) {
                packAsset = assetPool[assetUrl];
                packAsset.addRef ();
                targetAsset = packAsset.targetAsset as T;
                return targetAsset;
            }

            targetAsset = Resources.Load<T> (assetUrl);
            PackAsset packageAsset = new PackAsset (targetAsset);
            assetPool.Add (assetUrl, packageAsset);
            return targetAsset;
        }

        [Obsolete ("unity不允许")]
        public async Task<T> getAssetByUrlAsyncOb<T> (string assetUrl) where T : Object {
            T targetAsset = null;
            PackAsset packAsset = null;
            if (assetPool.ContainsKey (assetUrl)) {
                packAsset = assetPool[assetUrl];
                packAsset.addRef ();
                targetAsset = packAsset.targetAsset as T;
                return targetAsset;
            }

            targetAsset = await Task.Run (() => {
                ResourceRequest request = Resources.LoadAsync<T> (assetUrl);
                return request.asset as T;
            });

            PackAsset packageAsset = new PackAsset (targetAsset);
            assetPool.Add (assetUrl, packageAsset);
            return targetAsset;
        }

        public void getAssetByUrlAsync<T> (string assetUrl, Action<T> callback) where T : Object {
            T targetAsset = null;
            PackAsset packAsset = null;
            if (assetPool.ContainsKey (assetUrl)) {
                packAsset = assetPool[assetUrl];
                packAsset.addRef ();
                targetAsset = packAsset.targetAsset as T;
                callback (targetAsset);
            }

            ResourceRequest request = Resources.LoadAsync<T> (assetUrl);

            request.completed += operation => {
                PackAsset packageAsset = new PackAsset (targetAsset);
                assetPool.Add (assetUrl, packageAsset);
                callback (request.asset as T);
            };

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
            if (releaseResult) {
                this.assetPool.Remove (assetUrl);
            }
            return releaseResult;
        }
    }
}