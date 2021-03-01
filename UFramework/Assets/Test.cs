using System.IO;
using System.Net.Mime;
/*
 * @Author: l hy 
 * @Date: 2021-01-21 22:15:59 
 * @Description: 用于各类测试项目
 * @Last Modified by: l hy
 * @Last Modified time: 2021-03-01 23:17:35
 */

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UFramework.GameCommon;
using UnityEditor;
using UnityEngine;
public class Test : MonoBehaviour {

    private AssetsManager assetManager = new AssetsManager ();

    public Transform nodeParent;

    private void Start () {
        // this.loadCube ();
        this.loadCubeCallback ();
        this.loadAllRes ();
        Debug.Log ("继续下一步");
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
        string bundleUrl = Application.dataPath + AssetUrl.bundleUrl;
        AssetsManager.instance.getAssetByBundleAsync<GameObject> (bundleUrl, "resbundle", "Cube", (GameObject cubeAsset) => {
            GameObject cube = Instantiate<GameObject> (cubeAsset);
            cube.transform.SetParent (nodeParent);
            cube.transform.localPosition = Vector3.zero;
        });
    }

}