using System;
using System.Collections.Generic;
namespace UFramework.GameCommon {
    using UnityEngine;
    public interface IAssetsManager {
        /// <summary>
        ///  加载Bundle清单文件
        /// </summary>
        void LoadManifestFile ();

        #region Resource 下资源加载接口
        T GetAssetByUrlSync<T> (string assetUrl) where T : Object;

        List<PackAsset> GetAllAssetsByUrlSync<T> (string folderUrl) where T : Object;

        void GetAssetByUrlAsync<T> (string assetUrl, Action<T> callback) where T : Object;

        bool TryReleaseAsset (string assetUrl);
        #endregion

        #region Bundle下加载资源接口

        T GetAssetByBundleSync<T> (string bundleName, string assetName) where T : Object;

        List<PackAsset> GetAllAssetsByBundleSync<T> (string bundleName) where T : Object;

        void GetAssetByBundleAsync<T> (string bundleName, string assetName, Action<T> callback) where T : Object;

        void GetAllAssetsByBundleASync<T> (string bundleName, Action<List<PackAsset>> callback) where T : Object;

        bool TryReleaseBundle (string bundleName, bool unloadAllLoadedObjects = false);

        #endregion
    }
}