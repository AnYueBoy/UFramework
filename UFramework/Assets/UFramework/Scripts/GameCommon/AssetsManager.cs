using System;
/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:56:04 
 * @Description: 资源访问的统一对外接口
 * @Last Modified by: l hy
 * @Last Modified time: 2021-02-23 21:46:25
 */
namespace UFramework.GameCommon {

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class AssetsManager {

        private Dictionary<string, PackAsset> assetPool = new Dictionary<string, PackAsset> ();

        private static AssetsManager _instance;
        public static AssetsManager instance {
            get {
                if (_instance == null) {
                    _instance = new AssetsManager ();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取指定资源
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T getAssetByUrlSync<T> (string assetUrl) where T : Object {
            T nativeAsset = this.findNativeAsset<T> (assetUrl);
            if (nativeAsset != null) {
                return nativeAsset;
            }

            T targetAsset = null;
            targetAsset = Resources.Load<T> (assetUrl);
            PackAsset packageAsset = new PackAsset (targetAsset);
            assetPool.Add (assetUrl, packageAsset);
            return targetAsset;
        }

        [Obsolete ("unity not allow")]
        public async Task<T> getAssetByUrlAsyncOb<T> (string assetUrl) where T : Object {
            T nativeAsset = this.findNativeAsset<T> (assetUrl);
            if (nativeAsset != null) {
                return nativeAsset;
            }

            T targetAsset = null;
            targetAsset = await Task.Run (() => {
                ResourceRequest request = Resources.LoadAsync<T> (assetUrl);
                PackAsset packageAsset = new PackAsset (request.asset);
                assetPool.Add (assetUrl, packageAsset);
                return request.asset as T;
            });

            return targetAsset;
        }

        public void getAssetByUrlAsync<T> (string assetUrl, Action<T> callback) where T : Object {
            T nativeAsset = this.findNativeAsset<T> (assetUrl);
            if (nativeAsset != null) {
                callback (nativeAsset);
                return;
            }

            ResourceRequest request = Resources.LoadAsync<T> (assetUrl);

            request.completed += operation => {
                PackAsset packageAsset = new PackAsset (request.asset);
                assetPool.Add (assetUrl, packageAsset);
                callback (request.asset as T);
            };
        }

        private T findNativeAsset<T> (string assetUrl) where T : Object {
            if (assetPool.ContainsKey (assetUrl)) {
                PackAsset packAsset = assetPool[assetUrl];
                packAsset.addRef ();
                return packAsset.targetAsset as T;
            }
            return null;
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