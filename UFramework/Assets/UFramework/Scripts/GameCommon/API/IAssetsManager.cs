using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace UFramework.GameCommon {
    using UnityEngine;
    public interface IAssetsManager {
        /// <summary>
        ///  加载Bundle清单文件
        /// </summary>
        void loadManifestFile ();

        #region Resource 下资源加载接口
        T getAssetByUrlSync<T> (string assetUrl) where T : Object;

        List<PackAsset> getAllAssetsByUrlSync<T> (string folderUrl) where T : Object;

        Task<T> getAssetByUrlAsyncOb<T> (string assetUrl) where T : Object;

        void getAssetByUrlAsync<T> (string assetUrl, Action<T> callback) where T : Object;

        bool tryReleaseAsset (string assetUrl);
        #endregion

        #region Bundle下加载资源接口

        T getAssetByBundleSync<T> (string bundleName, string assetName) where T : Object;

        List<PackAsset> getAllAssetsByBundleSync<T> (string bundleName) where T : Object;

        void getAssetByBundleAsync<T> (string bundleName, string assetName, Action<T> callback) where T : Object;

        void getAllAssetsByBundleASync<T> (string bundleName, Action<List<PackAsset>> callback) where T : Object;

        bool tryReleaseBundle (string bundleName, bool unloadAllLoadedObjects = false);

        #endregion
    }
}