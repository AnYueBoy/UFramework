/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:56:04 
 * @Description: 资源访问的统一对外接口
 * @Last Modified by: l hy
 * @Last Modified time: 2022-01-13 19:35:56
 */
using System;
using SException = System.Exception;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UFramework.FrameUtil;
namespace UFramework.GameCommon {
    using UFramework.Promise;
    using UnityEngine;

    public class AssetsManager : IAssetsManager {

        private Dictionary<string, PackAsset> assetPool = new Dictionary<string, PackAsset> ();

        private Dictionary<string, List<PackAsset>> floderAssetPool = new Dictionary<string, List<PackAsset>> ();

        private Dictionary<string, AssetBundle> bundleDic = new Dictionary<string, AssetBundle> ();
        private Dictionary<string, Promise<AssetBundle>> bundleMap = new Dictionary<string, Promise<AssetBundle>> ();

        private AssetBundleManifest assetBundleManifest = null;

        public void LoadManifestFile () {
            if (this.assetBundleManifest != null) {
                return;
            }
            string manifestFileBundleUrl = CommonUtil.getBundleUrl () + CommonUtil.getCurPlatformName ();
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
        public T GetAssetByUrlSync<T> (string assetUrl) where T : Object {
            T nativeAsset = this.findNativeAsset<T> (assetUrl);
            if (nativeAsset != null) {
                return nativeAsset;
            }

            T targetAsset = null;
            targetAsset = Resources.Load<T> (assetUrl);
            PackAsset packageAsset = new PackAsset (assetUrl, targetAsset);
            assetPool.Add (assetUrl, packageAsset);
            return targetAsset;
        }

        /// <summary>
        /// 加载文件夹下的所有资源
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<PackAsset> GetAllAssetsByUrlSync<T> (string folderUrl) where T : Object {
            List<PackAsset> nativeAssets = this.findNativeAssets (folderUrl);
            if (nativeAssets != null) {
                return nativeAssets;
            }

            T[] targetAssets = Resources.LoadAll<T> (folderUrl);
            nativeAssets = new List<PackAsset> ();
            bool isContainSplash = folderUrl[folderUrl.Length - 1] == '/';
            if (!isContainSplash) {
                folderUrl = folderUrl + "/";
            }
            foreach (T targetAsset in targetAssets) {
                string assetUrl = folderUrl + targetAsset.name;
                PackAsset packAsset = new PackAsset (assetUrl, targetAsset);
                nativeAssets.Add (packAsset);
                assetPool.Add (assetUrl, packAsset);
            }
            floderAssetPool.Add (folderUrl, nativeAssets);
            return nativeAssets;
        }

        // [Obsolete ("unity not allow")]
        // public async Task<T> GetAssetByUrlAsyncOb<T> (string assetUrl) where T : Object {
        //     T nativeAsset = this.findNativeAsset<T> (assetUrl);
        //     if (nativeAsset != null) {
        //         return nativeAsset;
        //     }

        //     T targetAsset = null;
        //     targetAsset = await Task.Run (() => {
        //         ResourceRequest request = Resources.LoadAsync<T> (assetUrl);
        //         PackAsset packageAsset = new PackAsset (assetUrl, request.asset);
        //         assetPool.Add (assetUrl, packageAsset);
        //         return request.asset as T;
        //     });

        //     return targetAsset;
        // }

        public void GetAssetByUrlAsync<T> (string assetUrl, Action<T> callback) where T : Object {
            T nativeAsset = this.findNativeAsset<T> (assetUrl);
            if (nativeAsset != null) {
                callback (nativeAsset);
                return;
            }

            ResourceRequest request = Resources.LoadAsync<T> (assetUrl);

            request.completed += operation => {
                PackAsset packageAsset = new PackAsset (assetUrl, request.asset);
                assetPool.Add (assetUrl, packageAsset);
                callback (request.asset as T);
            };
        }

        private T findNativeAsset<T> (string assetUrl) where T : Object {
            if (assetPool.ContainsKey (assetUrl)) {
                // FIXME: 拿到资源不一定使用
                PackAsset packAsset = assetPool[assetUrl];
                return packAsset.TargetAsset as T;
            }
            return null;
        }

        private List<PackAsset> findNativeAssets (string assetUrl) {
            if (floderAssetPool.ContainsKey (assetUrl)) {
                return this.floderAssetPool[assetUrl];
            }
            return null;
        }

        /// <summary>
        /// 尝试释放资源并返回释放结果
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <returns></returns>
        public bool TryReleaseAsset (string assetUrl) {
            if (!this.assetPool.ContainsKey (assetUrl)) {
                Debug.LogWarning ("can not release not exist asset");
                return false;
            }

            PackAsset packAsset = this.assetPool[assetUrl];
            bool releaseResult = packAsset.ReleaseAsset ();
            if (releaseResult) {
                this.assetPool.Remove (assetUrl);
            }
            return releaseResult;
        }

        #endregion

        #region Asset Bundle Load Asset
        public T GetAssetByBundleSync<T> (string bundleName, string assetName) where T : Object {
            string nativeUrl = bundleName + "/" + assetName;
            T nativeAsset = this.findNativeAsset<T> (nativeUrl);
            if (nativeAsset != null) {
                return nativeAsset;
            }

            // 检查依赖资源
            this.checkDependenciesSync (bundleName);

            string bundleUrl = CommonUtil.getBundleUrl ();

            string targetBundleUrl = bundleUrl + bundleName;

            AssetBundle targetBundle = this.loadTargetBundleSync (targetBundleUrl);

            return this.loadTargetBundleAssetSync<T> (targetBundle, assetName);
        }

        /// <summary>
        /// 同步获取目标bundle下的所有资源
        /// </summary>
        /// <param name="bundleUrl"></param>
        /// <param name="bundleName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<PackAsset> GetAllAssetsByBundleSync<T> (string bundleName) where T : Object {
            List<PackAsset> nativeAssets = this.findNativeAssets (bundleName);
            if (nativeAssets != null) {
                return nativeAssets;
            }

            // 检查依赖资源
            this.checkDependenciesSync (bundleName);

            string bundleUrl = CommonUtil.getBundleUrl ();
            string targetBundleUrl = bundleUrl + bundleName;

            AssetBundle targetBundle = this.loadTargetBundleSync (targetBundleUrl);
            return this.loadTaregetBundleAllAssetSync<T> (targetBundle);
        }

        public void GetAssetByBundleAsync<T> (string bundleName, string assetName, Action<T> callback) where T : Object {
            string nativeUrl = bundleName + "/" + assetName;
            T nativeAsset = this.findNativeAsset<T> (nativeUrl);
            if (nativeAsset != null) {
                callback (nativeAsset);
                return;
            }

            // 异步依赖检查
            this.checkDependenciesAsync (bundleName)
                .Then (() => {
                    string bundleUrl = CommonUtil.getBundleUrl ();
                    string targetBundleUrl = bundleUrl + bundleName;
                    this.loadTargetBundleAsync (targetBundleUrl)
                        .Then ((AssetBundle targetBundle) => {
                            this.loadTargetBundleAssetAsync<T> (targetBundle, assetName)
                                .Then ((T targetAsset) => {
                                    callback (targetAsset);
                                });
                        });
                });
        }

        /// <summary>
        /// 异步加载某bundle下的所有资源
        /// </summary>
        /// <param name="bundleUrl"></param>
        /// <param name="bundleName"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public void GetAllAssetsByBundleASync<T> (string bundleName, Action<List<PackAsset>> callback) where T : Object {
            List<PackAsset> nativeAssets = this.findNativeAssets (bundleName);
            if (nativeAssets != null) {
                callback?.Invoke (nativeAssets);
                return;
            }

            // 异步依赖检查
            this.checkDependenciesAsync (bundleName)
                .Then (() => {
                    string bundleUrl = CommonUtil.getBundleUrl ();
                    string targetBundleUrl = bundleUrl + bundleName;
                    this.loadTargetBundleAsync (targetBundleUrl)
                        .Then ((AssetBundle targetBundle) => {
                            this.loadTargetBundleAllAssetAsync<T> (targetBundle)
                                .Then ((List<PackAsset> targetAssets) => {
                                    callback (targetAssets);
                                });
                        });
                });
        }

        public bool TryReleaseBundle (string bundleName, bool unloadAllLoadedObjects = false) {
            string bundleUrl = CommonUtil.getBundleUrl ();
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
        /// 同步递归检查依赖项
        /// </summary>
        /// <param name="bundleName"></param>
        private void checkDependenciesSync (string bundleName) {
            this.LoadManifestFile ();
            string[] allDependencies = this.assetBundleManifest.GetAllDependencies (bundleName);
            if (allDependencies.Length <= 0) {
                string bundleUrl = CommonUtil.getBundleUrl () + bundleName;
                this.loadTargetBundleSync (bundleUrl);
                return;
            }
            foreach (string dependenceBundleName in allDependencies) {
                this.checkDependenciesSync (dependenceBundleName);
            }
        }

        /* 异步检查依赖项 */
        private Promise checkDependenciesAsync (string bundleName) {
            List<Promise> allPromise = new List<Promise> ();
            this.addAllDependenciesBundle (allPromise, bundleName);
            return Promise.All (allPromise.ToArray ());
        }

        private void addAllDependenciesBundle (List<Promise> promiseList, string bundleName) {
            this.LoadManifestFile ();
            string[] allDependencies = this.assetBundleManifest.GetAllDependencies (bundleName);
            if (allDependencies.Length <= 0) {
                promiseList.Add (new Promise ((Action reslove, Action<SException> reject) => {
                    string bundleUrl = CommonUtil.getBundleUrl () + bundleName;
                    this.loadTargetBundleAsync (bundleUrl).Then (
                        (AssetBundle assetBundle) => {
                            reslove ();
                        }
                    );
                }));
                return;
            }

            foreach (string dependenceBundleName in allDependencies) {
                this.addAllDependenciesBundle (promiseList, dependenceBundleName);
            }
        }

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
            string assetUrl = targetBundle.name + ":" + assetName;
            PackAsset packAsset = new PackAsset (assetUrl, nativeAsset);
            string nativeUrl = targetBundle.name + "/" + assetName;
            this.assetPool.Add (nativeUrl, packAsset);
            return nativeAsset as T;
        }

        /*同步加载目标AB包下的所有资源*/
        private List<PackAsset> loadTaregetBundleAllAssetSync<T> (AssetBundle targetBundle) where T : Object {
            T[] targetAssetArray = targetBundle.LoadAllAssets<T> ();
            List<PackAsset> nativeAssetList = new List<PackAsset> ();
            foreach (T targetAsset in targetAssetArray) {
                string assetUrl = targetBundle.name + ":" + targetAsset.name;
                PackAsset packAsset = new PackAsset (assetUrl, targetAsset);
                nativeAssetList.Add (packAsset);
                string nativeUrl = targetBundle.name + "/" + targetAsset.name;
                assetPool.Add (nativeUrl, packAsset);
            }
            floderAssetPool.Add (targetBundle.name, nativeAssetList);
            return nativeAssetList;
        }

        /* 异步加载目标AB包 */
        private Promise<AssetBundle> loadTargetBundleAsync (string bundleUrl) {
            if (this.bundleDic.ContainsKey (bundleUrl)) {
                AssetBundle targetAssetBundle = this.bundleDic[bundleUrl];
                return Promise<AssetBundle>.Resolved (targetAssetBundle);
            }

            if (this.bundleMap.ContainsKey (bundleUrl)) {
                return this.bundleMap[bundleUrl];
            }

            this.bundleMap.Add (bundleUrl, new Promise<AssetBundle> ((Action<AssetBundle> resolve, Action<SException> reject) => {
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
                (Action<T> resolve, Action<SException> reject) => {
                    assetBundleRequest.completed += bundleOperation => {
                        T nativeAsset = assetBundleRequest.asset as T;
                        string assetUrl = targetBundle.name + ":" + assetName;
                        PackAsset packAsset = new PackAsset (assetUrl, nativeAsset);
                        string nativeUrl = targetBundle.name + "/" + assetName;
                        if (this.assetPool.ContainsKey (nativeUrl)) {
                            this.assetPool.Remove (nativeUrl);
                        }
                        this.assetPool.Add (nativeUrl, packAsset);
                        resolve (nativeAsset);
                    };
                }
            );
        }

        /*异步加载目标AB下的所有资源*/
        private Promise<List<PackAsset>> loadTargetBundleAllAssetAsync<T> (AssetBundle targetBundle) where T : Object {
            AssetBundleRequest assetBundleRequest = targetBundle.LoadAllAssetsAsync<T> ();
            return new Promise<List<PackAsset>> (
                (Action<List<PackAsset>> resolve, Action<SException> reject) => {
                    assetBundleRequest.completed += bundleOperation => {
                        Object[] allAssets = assetBundleRequest.allAssets;
                        List<PackAsset> nativeAssetList = new List<PackAsset> ();
                        foreach (Object targetAsset in allAssets) {
                            string assetUrl = targetBundle.name + ":" + targetAsset.name;
                            PackAsset packAsset = new PackAsset (assetUrl, targetAsset);
                            nativeAssetList.Add (packAsset);
                            string nativeUrl = targetBundle.name + "/" + targetAsset.name;
                            assetPool.Add (nativeUrl, packAsset);
                        }
                        floderAssetPool.Add (targetBundle.name, nativeAssetList);
                        resolve (nativeAssetList);
                    };
                }
            );
        }

        #endregion
    }
}