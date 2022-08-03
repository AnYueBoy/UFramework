/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:56:04 
 * @Description: 资源访问的统一对外接口
 * @Last Modified by: l hy
 * @Last Modified time: 2022-01-13 19:35:56
 */

using System;
using System.Collections.Generic;
using System.IO;
using UFramework.FrameUtil;
using UFramework.Promise;
using UnityEngine;
using Object = UnityEngine.Object;
using SException = System.Exception;

namespace UFramework.GameCommon
{
    public class AssetsManager : IAssetsManager
    {
        private Dictionary<string, PackAsset> assetPool = new Dictionary<string, PackAsset>();

        private Dictionary<string, List<PackAsset>> floderAssetPool = new Dictionary<string, List<PackAsset>>();

        private Dictionary<string, AssetBundle> bundleDic = new Dictionary<string, AssetBundle>();
        private Dictionary<string, Promise<AssetBundle>> bundleMap = new Dictionary<string, Promise<AssetBundle>>();

        private AssetBundleManifest assetBundleManifest = null;

        public void LoadManifestFile()
        {
            if (assetBundleManifest != null)
            {
                return;
            }

            string manifestFileBundleUrl = CommonUtil.GetBundleUrl() + CommonUtil.GetCurPlatformName();
            AssetBundle manifestFileBundle = AssetBundle.LoadFromFile(manifestFileBundleUrl);
            assetBundleManifest = manifestFileBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        #region Resources Load Asset

        /// <summary>
        /// 获取指定资源
        /// </summary>
        public T GetAssetByUrlSync<T>(string assetUrl) where T : Object
        {
            T nativeAsset = FindNativeAsset<T>(assetUrl);
            if (nativeAsset != null)
            {
                return nativeAsset;
            }

            T targetAsset = Resources.Load<T>(assetUrl);
            if (targetAsset == null)
            {
                Debug.LogError($"Get {assetUrl} not exist.");
                return null;
            }

            PackAsset packageAsset = new PackAsset(assetUrl, targetAsset);
            assetPool.Add(assetUrl, packageAsset);
            return targetAsset;
        }

        /// <summary>
        /// 加载文件夹下的所有资源
        /// </summary>
        public List<PackAsset> GetAllAssetsByUrlSync<T>(string folderUrl) where T : Object
        {
            List<PackAsset> nativeAssets = FindNativeAssets(folderUrl);
            if (nativeAssets != null)
            {
                return nativeAssets;
            }

            T[] targetAssets = Resources.LoadAll<T>(folderUrl);
            nativeAssets = new List<PackAsset>();
            bool isContainSplash = folderUrl[folderUrl.Length - 1] == '/';
            if (!isContainSplash)
            {
                folderUrl = folderUrl + "/";
            }

            foreach (T targetAsset in targetAssets)
            {
                string assetUrl = folderUrl + targetAsset.name;
                PackAsset packAsset = new PackAsset(assetUrl, targetAsset);
                nativeAssets.Add(packAsset);
                assetPool.Add(assetUrl, packAsset);
            }

            floderAssetPool.Add(folderUrl, nativeAssets);
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

        public void GetAssetByUrlAsync<T>(string assetUrl, Action<T> callback) where T : Object
        {
            T nativeAsset = FindNativeAsset<T>(assetUrl);
            if (nativeAsset != null)
            {
                callback?.Invoke(nativeAsset);
                return;
            }

            ResourceRequest request = Resources.LoadAsync<T>(assetUrl);

            request.completed += operation =>
            {
                PackAsset packageAsset = new PackAsset(assetUrl, request.asset);
                assetPool.Add(assetUrl, packageAsset);
                callback?.Invoke(request.asset as T);
            };
        }

        public Promise<List<T>> GetAssetsByListAsync<T>(params string[] assetsUrlList) where T : Object
        {
            List<Promise<T>> allPromise = new List<Promise<T>>();
            List<T> assetsList = new List<T>();
            foreach (string url in assetsUrlList)
            {
                allPromise.Add(new Promise<T>((resolve, reject) =>
                {
                    GetAssetByUrlAsync(url, (T asset) =>
                    {
                        assetsList.Add(asset);
                        resolve?.Invoke(asset);
                    });
                }));
            }

            return new Promise<List<T>>((resolve, reject) =>
            {
                Promise<T>.All(allPromise.ToArray())
                    .Then(objects => { resolve?.Invoke(assetsList); })
                    .Catchs((
                        exception =>
                        {
                            Debug.LogWarning($"exception {exception}");
                            reject?.Invoke(exception);
                        }));
            });
        }

        private T FindNativeAsset<T>(string assetUrl) where T : Object
        {
            if (assetPool.ContainsKey(assetUrl))
            {
                PackAsset packAsset = assetPool[assetUrl];
                return packAsset.TargetAsset as T;
            }

            return null;
        }

        private List<PackAsset> FindNativeAssets(string assetUrl)
        {
            if (floderAssetPool.ContainsKey(assetUrl))
            {
                return floderAssetPool[assetUrl];
            }

            return null;
        }

        /// <summary>
        /// 尝试释放资源
        /// </summary>
        public void TryReleaseAsset(string assetUrl)
        {
            if (!assetPool.ContainsKey(assetUrl))
            {
                Debug.LogWarning("can not release not exist asset");
                return;
            }

            PackAsset packAsset = assetPool[assetUrl];
            packAsset.ReleaseAsset();
            assetPool.Remove(assetUrl);
        }

        #endregion

        #region Asset Bundle Load Asset

        public T GetAssetByBundleSync<T>(string bundleName, string assetName) where T : Object
        {
            string nativeUrl = bundleName + "/" + assetName;
            T nativeAsset = FindNativeAsset<T>(nativeUrl);
            if (nativeAsset != null)
            {
                return nativeAsset;
            }

            // 检查依赖资源
            CheckDependenciesSync(bundleName);

            string bundleUrl = CommonUtil.GetBundleUrl();

            string targetBundleUrl = bundleUrl + bundleName;

            AssetBundle targetBundle = LoadTargetBundleSync(targetBundleUrl);

            return LoadTargetBundleAssetSync<T>(targetBundle, assetName);
        }

        /// <summary>
        /// 同步获取目标bundle下的所有资源
        /// </summary>
        public List<PackAsset> GetAllAssetsByBundleSync<T>(string bundleName) where T : Object
        {
            List<PackAsset> nativeAssets = FindNativeAssets(bundleName);
            if (nativeAssets != null)
            {
                return nativeAssets;
            }

            // 检查依赖资源
            CheckDependenciesSync(bundleName);

            string bundleUrl = CommonUtil.GetBundleUrl();
            string targetBundleUrl = bundleUrl + bundleName;

            AssetBundle targetBundle = LoadTargetBundleSync(targetBundleUrl);
            return LoadTargetBundleAllAssetSync<T>(targetBundle);
        }

        public void GetAssetByBundleAsync<T>(string bundleName, string assetName, Action<T> callback) where T : Object
        {
            string nativeUrl = bundleName + "/" + assetName;
            T nativeAsset = FindNativeAsset<T>(nativeUrl);
            if (nativeAsset != null)
            {
                callback(nativeAsset);
                return;
            }

            // 异步依赖检查
            CheckDependenciesAsync(bundleName)
                .Then(() =>
                {
                    string bundleUrl = CommonUtil.GetBundleUrl();
                    string targetBundleUrl = bundleUrl + bundleName;
                    LoadTargetBundleAsync(targetBundleUrl)
                        .Then((AssetBundle targetBundle) =>
                        {
                            LoadTargetBundleAssetAsync<T>(targetBundle, assetName)
                                .Then((T targetAsset) => { callback(targetAsset); });
                        });
                });
        }

        /// <summary>
        /// 异步加载某bundle下的所有资源
        /// </summary>
        public void GetAllAssetsByBundleASync<T>(string bundleName, Action<List<PackAsset>> callback) where T : Object
        {
            List<PackAsset> nativeAssets = FindNativeAssets(bundleName);
            if (nativeAssets != null)
            {
                callback?.Invoke(nativeAssets);
                return;
            }

            // 异步依赖检查
            CheckDependenciesAsync(bundleName)
                .Then(() =>
                {
                    string bundleUrl = CommonUtil.GetBundleUrl();
                    string targetBundleUrl = bundleUrl + bundleName;
                    LoadTargetBundleAsync(targetBundleUrl)
                        .Then((AssetBundle targetBundle) =>
                        {
                            LoadTargetBundleAllAssetAsync<T>(targetBundle)
                                .Then((List<PackAsset> targetAssets) => { callback(targetAssets); });
                        });
                });
        }

        public void TryReleaseBundle(string bundleName, bool unloadAllLoadedObjects = false)
        {
            string bundleUrl = CommonUtil.GetBundleUrl();
            string targetBundleUrl = Path.Combine(bundleUrl, bundleName);
            if (!assetPool.ContainsKey(targetBundleUrl))
            {
                Debug.LogWarning("can not release not exist bundle");
                return;
            }

            AssetBundle targetBundle = bundleDic[targetBundleUrl];
            targetBundle.Unload(unloadAllLoadedObjects);
        }

        /// <summary>
        /// 同步递归检查依赖项
        /// </summary>
        private void CheckDependenciesSync(string bundleName)
        {
            LoadManifestFile();
            string[] allDependencies = assetBundleManifest.GetAllDependencies(bundleName);
            if (allDependencies.Length <= 0)
            {
                string bundleUrl = CommonUtil.GetBundleUrl() + bundleName;
                LoadTargetBundleSync(bundleUrl);
                return;
            }

            foreach (string dependenceBundleName in allDependencies)
            {
                CheckDependenciesSync(dependenceBundleName);
            }
        }

        /// <summary>
        /// 异步检查依赖项
        /// </summary>
        private Promise.Promise CheckDependenciesAsync(string bundleName)
        {
            List<Promise.Promise> allPromise = new List<Promise.Promise>();
            AddAllDependenciesBundle(allPromise, bundleName);
            return Promise.Promise.All(allPromise.ToArray());
        }

        private void AddAllDependenciesBundle(List<Promise.Promise> promiseList, string bundleName)
        {
            LoadManifestFile();
            string[] allDependencies = assetBundleManifest.GetAllDependencies(bundleName);
            if (allDependencies.Length <= 0)
            {
                promiseList.Add(new Promise.Promise((Action resolve, Action<SException> reject) =>
                {
                    string bundleUrl = CommonUtil.GetBundleUrl() + bundleName;
                    LoadTargetBundleAsync(bundleUrl).Then(
                        (AssetBundle assetBundle) => { resolve(); }
                    );
                }));
                return;
            }

            foreach (string dependenceBundleName in allDependencies)
            {
                AddAllDependenciesBundle(promiseList, dependenceBundleName);
            }
        }

        /// <summary>
        /// 同步加载目标AB包
        /// </summary>
        private AssetBundle LoadTargetBundleSync(string bundleUrl)
        {
            if (!bundleDic.ContainsKey(bundleUrl))
            {
                AssetBundle targetAssetBundle = AssetBundle.LoadFromFile(bundleUrl);
                bundleDic.Add(bundleUrl, targetAssetBundle);
            }

            return bundleDic[bundleUrl];
        }

        /// <summary>
        /// 同步加载目标AB包下的对应资源
        /// </summary>
        private T LoadTargetBundleAssetSync<T>(AssetBundle targetBundle, string assetName) where T : Object
        {
            T nativeAsset = targetBundle.LoadAsset<T>(assetName);
            string assetUrl = targetBundle.name + ":" + assetName;
            PackAsset packAsset = new PackAsset(assetUrl, nativeAsset);
            string nativeUrl = targetBundle.name + "/" + assetName;
            assetPool.Add(nativeUrl, packAsset);
            return nativeAsset as T;
        }

        /// <summary>
        /// 同步加载目标AB包下的所有资源
        /// </summary>
        private List<PackAsset> LoadTargetBundleAllAssetSync<T>(AssetBundle targetBundle) where T : Object
        {
            T[] targetAssetArray = targetBundle.LoadAllAssets<T>();
            List<PackAsset> nativeAssetList = new List<PackAsset>();
            foreach (T targetAsset in targetAssetArray)
            {
                string assetUrl = targetBundle.name + ":" + targetAsset.name;
                PackAsset packAsset = new PackAsset(assetUrl, targetAsset);
                nativeAssetList.Add(packAsset);
                string nativeUrl = targetBundle.name + "/" + targetAsset.name;
                assetPool.Add(nativeUrl, packAsset);
            }

            floderAssetPool.Add(targetBundle.name, nativeAssetList);
            return nativeAssetList;
        }

        /// <summary>
        /// 异步加载目标AB包
        /// </summary>
        private Promise<AssetBundle> LoadTargetBundleAsync(string bundleUrl)
        {
            if (bundleDic.ContainsKey(bundleUrl))
            {
                AssetBundle targetAssetBundle = bundleDic[bundleUrl];
                return Promise<AssetBundle>.Resolved(targetAssetBundle);
            }

            if (bundleMap.ContainsKey(bundleUrl))
            {
                return bundleMap[bundleUrl];
            }

            bundleMap.Add(bundleUrl, new Promise<AssetBundle>(
                (Action<AssetBundle> resolve, Action<SException> reject) =>
                {
                    AssetBundleCreateRequest bundleCreateRequest = AssetBundle.LoadFromFileAsync(bundleUrl);
                    bundleCreateRequest.completed += (AsyncOperation operation) =>
                    {
                        AssetBundle assetBundle = bundleCreateRequest.assetBundle;
                        bundleDic.Add(bundleUrl, assetBundle);
                        resolve(assetBundle);
                        bundleMap.Remove(bundleUrl);
                    };
                }));

            return bundleMap[bundleUrl];
        }

        /// <summary>
        /// 异步加载目标AB下的对应资源
        /// </summary>
        private Promise<T> LoadTargetBundleAssetAsync<T>(AssetBundle targetBundle, string assetName) where T : Object
        {
            AssetBundleRequest assetBundleRequest = targetBundle.LoadAssetAsync<T>(assetName);
            return new Promise<T>(
                (Action<T> resolve, Action<SException> reject) =>
                {
                    assetBundleRequest.completed += bundleOperation =>
                    {
                        T nativeAsset = assetBundleRequest.asset as T;
                        string assetUrl = targetBundle.name + ":" + assetName;
                        PackAsset packAsset = new PackAsset(assetUrl, nativeAsset);
                        string nativeUrl = targetBundle.name + "/" + assetName;
                        if (assetPool.ContainsKey(nativeUrl))
                        {
                            assetPool.Remove(nativeUrl);
                        }

                        assetPool.Add(nativeUrl, packAsset);
                        resolve(nativeAsset);
                    };
                }
            );
        }

        /// <summary>
        /// 异步加载目标AB下的所有资源
        /// </summary>
        private Promise<List<PackAsset>> LoadTargetBundleAllAssetAsync<T>(AssetBundle targetBundle) where T : Object
        {
            AssetBundleRequest assetBundleRequest = targetBundle.LoadAllAssetsAsync<T>();
            return new Promise<List<PackAsset>>(
                (Action<List<PackAsset>> resolve, Action<SException> reject) =>
                {
                    assetBundleRequest.completed += bundleOperation =>
                    {
                        Object[] allAssets = assetBundleRequest.allAssets;
                        List<PackAsset> nativeAssetList = new List<PackAsset>();
                        foreach (Object targetAsset in allAssets)
                        {
                            string assetUrl = targetBundle.name + ":" + targetAsset.name;
                            PackAsset packAsset = new PackAsset(assetUrl, targetAsset);
                            nativeAssetList.Add(packAsset);
                            string nativeUrl = targetBundle.name + "/" + targetAsset.name;
                            assetPool.Add(nativeUrl, packAsset);
                        }

                        floderAssetPool.Add(targetBundle.name, nativeAssetList);
                        resolve(nativeAssetList);
                    };
                }
            );
        }

        #endregion
    }
}