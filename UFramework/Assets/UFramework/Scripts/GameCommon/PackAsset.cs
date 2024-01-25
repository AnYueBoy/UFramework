/*
 * @Author: l hy
 * @Date: 2021-01-18 22:00:38
 * @Description: 封装资源
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-21 22:26:24
 */

using UnityEngine;

namespace UFramework
{
    public class PackAsset
    {
        private string assetUrl;
        private Object targetAsset;

        public PackAsset(string assetUrl, Object targetAsset)
        {
            this.assetUrl = assetUrl;
            this.targetAsset = targetAsset;
        }

        public Object TargetAsset => targetAsset;
        public string AssetUrl => assetUrl;

        public void ReleaseAsset()
        {
            // UnloadAsset 不能卸载 GameObject、Component和AssetBundle 这三种资源
            if (TargetAsset is GameObject)
            {
                Resources.UnloadUnusedAssets();
                return;
            }

            Resources.UnloadAsset(TargetAsset);
        }
    }
}