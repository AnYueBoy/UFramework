/*
 * @Author: l hy 
 * @Date: 2021-01-18 22:00:38 
 * @Description: 封装资源
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-18 22:14:44
 */
namespace UFramework.GameCommon {
    using UnityEngine;
    public class PackAsset {

        public Object targetAsset;

        private int referenceCounter = 0;

        public PackAsset (Object targetAsset) {
            this.targetAsset = targetAsset;
            this.addRef ();
        }

        public void addRef () {
            this.referenceCounter++;
        }

        public void reduceRef () {
            this.referenceCounter--;
            if (this.referenceCounter <= 0) {
                this.releaseAsset ();
            }
        }

        public bool releaseAsset () {
            if (this.referenceCounter > 0) {
                return false;
            }

            Resources.UnloadAsset (this.targetAsset);
            return true;
        }

    }
}