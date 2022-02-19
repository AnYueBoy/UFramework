/*
 * @Author: l hy 
 * @Date: 2021-01-18 22:00:38 
 * @Description: 封装资源
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-21 22:26:24
 */
using UnityEngine;
namespace UFramework.GameCommon {
    public class PackAsset {

        public string assetUrl;
        private Object _targetAsset;

        private int referenceCounter = 0;

        public PackAsset (string assetUrl, Object targetAsset) {
            this.assetUrl = assetUrl;
            this._targetAsset = targetAsset;
        }

        public Object TargetAsset {
            get {
                this.AddRef ();
                return this._targetAsset;
            }
        }

        private void AddRef () {
            this.referenceCounter++;
        }

        public bool ReleaseAsset () {
            this.referenceCounter--;
            if (this.referenceCounter > 0) {
                return false;
            }

            // UnloadAsset 不能卸载 GameObject、Component和AssetBundle 这三种资源
            if (this.TargetAsset is GameObject) {
                Resources.UnloadUnusedAssets ();
                return true;
            }
            Resources.UnloadAsset (this.TargetAsset);
            return true;
        }

    }
}