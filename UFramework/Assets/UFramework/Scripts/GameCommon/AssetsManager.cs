/*
 * @Author: l hy 
 * @Date: 2020-10-10 06:56:04 
 * @Description: 资源访问的统一对外接口
 * @Last Modified by: l hy
 * @Last Modified time: 2020-10-10 07:27:03
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManager {

    private Dictionary<string, Object> assetsPool = new Dictionary<string, Object> ();

    private Dictionary<string, int> referenceCounter = new Dictionary<string, int> ();

    public T getAssetsByUrl<T> (string assetsUrl) where T : Object {
        Object targetAssets = null;
        this.addRef (assetsUrl);
        if (assetsPool.ContainsKey (assetsUrl)) {
            targetAssets = assetsPool[assetsUrl];
            return targetAssets as T;
        }

        targetAssets = AssetsLoadManager.loadAssets<T> (assetsUrl);
        assetsPool.Add (assetsUrl, targetAssets);

        return targetAssets as T;
    }

    // FIXME: 等待完善
    // public int unLoadAssetByUrl (string assetsUrl) {
    //     if (!referenceCounter.ContainsKey (assetsUrl)) {
    //         Resources.UnloadUnusedAssets ();
    //     }
    // }

    private void addRef (string assetsUrl) {
        if (referenceCounter.ContainsKey (assetsUrl)) {
            referenceCounter[assetsUrl]++;
        } else {
            referenceCounter.Add (assetsUrl, 1);
        }
    }
}