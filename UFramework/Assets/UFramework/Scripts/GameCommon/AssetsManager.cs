/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:56:04 
 * @Description: 资源访问的统一对外接口
 * @Last Modified by: l hy
 * @Last Modified time: 2020-10-10 07:27:03
 */

using System.Collections.Generic;
using UnityEngine;

public class AssetsManager {

    private Dictionary<string, Object> assetsPool = new Dictionary<string, Object> ();

    private Dictionary<string, int> referenceCounter = new Dictionary<string, int> ();

    public T getAssetsByUrl<T> (string assetsUrl) where T : Object {
        Object targetAssets = null;
        if (assetsPool.ContainsKey (assetsUrl)) {
            targetAssets = assetsPool[assetsUrl];
            return targetAssets as T;
        }

        targetAssets = AssetsLoadManager.loadAssets<T> (assetsUrl);
        assetsPool.Add (assetsUrl, targetAssets);
        this.addRef (assetsUrl);
        return targetAssets as T;
    }

    public void tryReleaseAssets (string assetsUrl) {
        int result = this.redRef (assetsUrl);
        if (result <= -1) {
            Debug.LogError ("release assets error assets not exist");
            return;
        }

        if (result == 0) {
            Object assets = assetsPool[assetsUrl];
            assetsPool.Remove (assetsUrl);
            Resources.UnloadAsset (assets);
            return;
        }

        Debug.LogWarning ("release assets error exist not zero ref");
    }

    private void addRef (string assetsUrl) {
        if (referenceCounter.ContainsKey (assetsUrl)) {
            referenceCounter[assetsUrl]++;
        } else {
            referenceCounter.Add (assetsUrl, 1);
        }
    }

    private int redRef (string assetsUrl) {
        if (!referenceCounter.ContainsKey (assetsUrl)) {
            return -1;
        }

        referenceCounter[assetsUrl]--;
        if (referenceCounter[assetsUrl] <= 0) {
            referenceCounter.Remove (assetsUrl);
            return 0;
        }

        return referenceCounter[assetsUrl];
    }
}