using System;
using System.Collections.Generic;
/*
 * @Author: l hy 
 * @Date: 2021-01-21 22:15:59 
 * @Description: 用于各类测试项目
 * @Last Modified by: l hy
 * @Last Modified time: 2021-03-01 23:17:35
 */

using System.Threading.Tasks;
using UFramework.FrameUtil;
using UFramework.GameCommon;
using UFramework.Promise;
using UFrameWork;
using UnityEngine;

public class Test : MonoBehaviour {

    private AssetsManager assetManager = new AssetsManager ();

    public Transform nodeParent;

    private void Start () {
        // this.loadCube ();
        this.loadCubeCallback ();
        this.loadAllRes ();
        Debug.Log ("继续下一步");

        ModuleManager.instance.promiseTimer.waitFor (3).then (
            () => {
                Debug.Log ("时间等待结束");
            }
        );
    }

    private async void loadCube () {
        // FIXME: unity 不允许，在unity中我们使用多线程时。用子线程调用主线程时。用到unity的东西时就会报如下的错误。
        GameObject cubePrefab = await assetManager.getAssetByUrlAsyncOb<GameObject> ("Shape/Cube");
        GameObject cubeNode = Instantiate<GameObject> (cubePrefab);
        cubeNode.transform.SetParent (this.gameObject.transform);
    }

    private void loadCubeCallback () {
        assetManager.getAssetByUrlAsync<GameObject> ("Shape/Cube", (res) => {
            GameObject cubeNode = Instantiate<GameObject> (res);
            cubeNode.transform.SetParent (this.gameObject.transform);
        });

    }

    private float assetTimer = 0;
    private bool releaseCompleted = false;

    private void Update () {
        if (releaseCompleted) {
            return;
        }
        assetTimer += Time.deltaTime;
        if (assetTimer < 2.0f) {
            return;
        }

        releaseCompleted = true;
        Debug.Log ("releaseComplete : " + releaseCompleted);

        bool result = assetManager.tryReleaseAsset ("Shape/Cube");
        if (result) {
            Debug.Log ("卸载成功");
        }
    }

    private async Task<bool> loadResOne () {
        await Task.Delay (1000);
        Debug.Log ("资源1异步加载完成");
        return true;
    }

    private async Task<bool> loadResTwo () {
        await Task.Delay (500);
        Debug.Log ("资源2异步加载完成");
        return true;
    }

    private async void loadAllRes () {
        Debug.Log ("准备加载");
        var firstTask = this.loadResOne ();
        var secondTask = this.loadResTwo ();
        await firstTask;
        await secondTask;
        Debug.Log ("资源全部加载完成");
    }

    public void loadBundle () {
        string bundleUrl = CommonUtil.getBundleUrl ();
        // AssetsManager.instance.getAssetByBundleAsync<GameObject> (bundleUrl, "resbundle", "Cube", (GameObject cubeAsset) => {
        //     GameObject cube = Instantiate<GameObject> (cubeAsset);
        //     cube.transform.SetParent (nodeParent);
        //     cube.transform.localPosition = Vector3.zero;
        // });

        // AssetsManager.instance.getAssetByBundleAsync<GameObject> (bundleUrl, "resbundle", "Cube", (GameObject cubeAsset) => {
        //     GameObject cube = Instantiate<GameObject> (cubeAsset);
        //     cube.transform.SetParent (nodeParent);
        //     cube.transform.localPosition = Vector3.zero;
        // });

        // test ();

        // 同步加载文件夹资源
        // List<PackAsset> assetList = AssetsManager.instance.getAllAssetsByUrlSync<Sprite> ("Textures/");
        // Sprite attackSprite = AssetsManager.instance.getAssetByUrlSync<Sprite> ("Textures/attack");

        // 使用bundle同步加载文件夹资源
        // List<PackAsset> assetList = AssetsManager.instance.getAllAssetsByBundleSync<Sprite> (bundleUrl, "texturesbundle");
        // Sprite attackSprite = AssetsManager.instance.getAssetByBundleSync<Sprite> (bundleUrl, "texturesbundle", "attack");

        // 使用bundle异步加载文件夹资源
        AssetsManager.instance.getAllAssetsByBundleASync<Sprite> (bundleUrl, "texturesbundle", (List<PackAsset> assetsArray) => {
            List<PackAsset> assets = assetsArray;
            Sprite attackSprite = AssetsManager.instance.getAssetByBundleSync<Sprite> (bundleUrl, "texturesbundle", "attack");
        });

    }

    private Promise loadPromise () {
        return new Promise (
            (Action resolve, Action<Exception> reject) => {
                resolve ();
            }
        );
    }

    private void test () {
        this.loadPromise ()
            .then (() => {
                Debug.Log ("over");
            });
    }
}