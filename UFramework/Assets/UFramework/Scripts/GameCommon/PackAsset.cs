/*
 * @Author: l hy 
 * @Date: 2021-01-18 22:00:38 
 * @Description: 封装资源
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-21 22:26:24
 */
namespace UFramework.GameCommon {
    using UnityEngine;
    public class PackAsset {

        public string assetUrl;
        private Object _targetAsset;

        private int referenceCounter = 0;

        public PackAsset (string assetUrl, Object targetAsset) {
            this.assetUrl = assetUrl;
            this._targetAsset = targetAsset;
        }

        public Object targetAsset {
            get {
                this.addRef ();
                return this._targetAsset;
            }
        }

        private void addRef () {
            this.referenceCounter++;
        }

        public bool releaseAsset () {
            this.referenceCounter--;
            if (this.referenceCounter > 0) {
                return false;
            }

            // UnloadAsset 不能卸载 GameObject、Component和AssetBundle 这三种资源
            if (this.targetAsset is GameObject) {
                Resources.UnloadUnusedAssets ();
                return true;
            }
            Resources.UnloadAsset (this.targetAsset);
            return true;
        }

    }
}