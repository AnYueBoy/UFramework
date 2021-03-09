﻿using System;

/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:56:04 
 * @Description: 资源访问的统一对外接口
 * @Last Modified by: l hy
 * @Last Modified time: 2021-03-01 23:18:24
 */
namespace UFramework.GameCommon {

    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using UFramework.Promise;
    using UnityEngine;
    public class AssetsManager {

        private Dictionary<string, PackAsset> assetPool = new Dictionary<string, PackAsset> ();

        private Dictionary<string, AssetBundle> bundleDic = new Dictionary<string, AssetBundle> ();
        private Dictionary<string, Promise<AssetBundle>> bundleMap = new Dictionary<string, Promise<AssetBundle>> ();

        private AssetBundleManifest assetBundleManifest = null;

        private static AssetsManager _instance;
        public static AssetsManager instance {
            get {
                if (_instance == null) {
                    _instance = new AssetsManager ();
                }
                return _instance;
            }
        }

        public void init () {
            this.loadManifestFile ();
        }

        private void loadManifestFile () {
            if (this.assetBundleManifest != null) {
                return;
            }
            string manifestFileBundleUrl = Application.dataPath + AssetUrl.bundleUrl + AssetUrl.bundleUrl;
            AssetBundle manifestFileBundle = AssetBundle.LoadFromFile (manifestFileBundleUrl);
            this.assetBundleManifest = manifestFileBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
        }

        #region Resources Load Asset
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
                // FIXME: 拿到资源不一定使用
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

        #endregion

        #region Asset Bundle Load Asset
        public T getAssetByBundleSync<T> (string bundleUrl, string bundleName, string assetName) where T : Object {
            T nativeAsset = this.findNativeAsset<T> (assetName);
            if (nativeAsset != null) {
                return nativeAsset;
            }

            // 检查依赖资源
            this.checkDependenciesSync (bundleName);

            string targetBundleUrl = bundleUrl + "/" + bundleName;

            AssetBundle targetBundle = this.loadTargetBundleSync (targetBundleUrl);

            return this.loadTargetBundleAssetSync<T> (targetBundle, assetName);
        }

        public void getAssetByBundleAsync<T> (string bundleUrl, string bundleName, string assetName, Action<T> callback) where T : Object {
            T nativeAsset = this.findNativeAsset<T> (assetName);
            if (nativeAsset != null) {
                callback (nativeAsset);
                return;
            }

            // FIXME: 异步依赖检查
            // this.checkDependenciesSync (bundleName);

            string targetBundleUrl = bundleUrl + "/" + bundleName;
            this.loadTargetBundleAsync (targetBundleUrl)
                .then ((AssetBundle targetBundle) => {
                    this.loadTargetBundleAssetAsync<T> (targetBundle, assetName)
                        .then ((T targetAsset) => {
                            callback (targetAsset);
                        });
                });
        }

        public bool tryReleaseBundle (string bundleUrl, string bundleName, bool unloadAllLoadedObjects = false) {
            string targetBundleUrl = Path.Combine (bundleUrl, bundleName);
            if (!this.assetPool.ContainsKey (targetBundleUrl)) {
                Debug.LogWarning ("can not release not exist bundle");
                return false;
            }

            AssetBundle targetBundle = this.bundleDic[targetBundleUrl];
            targetBundle.Unload (unloadAllLoadedObjects);
            return true;
        }

        /// <summary>
        /// 递归检查依赖项
        /// </summary>
        /// <param name="bundleName"></param>
        private void checkDependenciesSync (string bundleName) {
            this.loadManifestFile ();
            string[] allDependencies = this.assetBundleManifest.GetAllDependencies (bundleName);
            if (allDependencies.Length <= 0) {
                string bundleUrl = Application.dataPath + AssetUrl.bundleUrl + "/" + bundleName;
                this.loadTargetBundleSync (bundleUrl);
                return;
            }
            foreach (string dependenceBundleName in allDependencies) {
                this.checkDependenciesSync (dependenceBundleName);
            }
        }

        // private Promise checkDependenciesAsync (string bundleName) {
        //     this.loadManifestFile ();
        //     string[] allDependencies = this.assetBundleManifest.GetAllDependencies (bundleName);
        // }

        /* 同步加载目标AB包 */
        private AssetBundle loadTargetBundleSync (string bundleUrl) {
            AssetBundle targetAssetBundle = null;
            if (!this.bundleDic.ContainsKey (bundleUrl)) {
                targetAssetBundle = AssetBundle.LoadFromFile (bundleUrl);
                this.bundleDic.Add (bundleUrl, targetAssetBundle);
            }

            return this.bundleDic[bundleUrl];
        }

        /* 同步加载目标AB包下的对应资源 */
        private T loadTargetBundleAssetSync<T> (AssetBundle targetBundle, string assetName) where T : Object {
            T nativeAsset = targetBundle.LoadAsset<T> (assetName);
            PackAsset packAsset = new PackAsset (nativeAsset);
            this.assetPool.Add (assetName, packAsset);
            return nativeAsset as T;
        }

        /* 异步加载目标AB包 */
        private Promise<AssetBundle> loadTargetBundleAsync (string bundleUrl) {
            if (this.bundleDic.ContainsKey (bundleUrl)) {
                AssetBundle targetAssetBundle = this.bundleDic[bundleUrl];
                return Promise<AssetBundle>.resolved (targetAssetBundle);
            }

            if (this.bundleMap.ContainsKey (bundleUrl)) {
                return this.bundleMap[bundleUrl];
            }

            this.bundleMap.Add (bundleUrl, new Promise<AssetBundle> ((Action<AssetBundle> resolve, Action<Exception> reject) => {
                AssetBundleCreateRequest bundleCreateRequest = AssetBundle.LoadFromFileAsync (bundleUrl);
                bundleCreateRequest.completed += (AsyncOperation operation) => {
                    AssetBundle assetBundle = bundleCreateRequest.assetBundle;
                    this.bundleDic.Add (bundleUrl, assetBundle);
                    resolve (assetBundle);
                    this.bundleMap.Remove (bundleUrl);
                };
            }));

            return this.bundleMap[bundleUrl];
        }

        /* 异步加载目标AB下的对应资源 */
        private Promise<T> loadTargetBundleAssetAsync<T> (AssetBundle targetBundle, string assetName) where T : Object {
            AssetBundleRequest assetBundleRequest = targetBundle.LoadAssetAsync<T> (assetName);
            return new Promise<T> (
                (Action<T> resolve, Action<Exception> reject) => {
                    assetBundleRequest.completed += bundleOperation => {
                        T nativeAsset = assetBundleRequest.asset as T;
                        PackAsset packAsset = new PackAsset (nativeAsset);
                        if (this.assetPool.ContainsKey (assetName)) {
                            this.assetPool.Remove (assetName);
                        }
                        this.assetPool.Add (assetName, packAsset);
                        resolve (nativeAsset);
                    };
                }
            );
        }

        #endregion
    }
}