/*
 * @Author: l hy 
 * @Date: 2021-01-21 22:15:59 
 * @Description: 用于各类测试项目
 * @Last Modified by: l hy
 * @Last Modified time: 2022-01-14 14:22:29
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UFramework.Core;
using UFramework.EventDispatcher;
using UFramework.FrameUtil;
using UFramework.GameCommon;
using UFramework.Promise;
using UnityEngine;

public class Test : MonoBehaviour {

    private AssetsManager assetManager = new AssetsManager ();

    private EventDispatcher eventDispatcher = new EventDispatcher ();

    public Transform nodeParent;

    private void Start () {
        // this.loadCube ();
        this.loadCubeCallback ();
        this.loadAllRes ();
        Debug.Log ("继续下一步");

        // ModuleManager.instance.promiseTimer.waitFor (3).then (
        //     () => {
        //         Debug.Log ("时间等待结束");
        //     }
        // );

        InjectTest injectTest = new InjectTest ();

        foreach (var property in injectTest.GetType ().GetProperties (BindingFlags.Public | BindingFlags.Instance)) {
            Debug.Log ($"{property.PropertyType}");
            Debug.Log ("");
        }

    }

    private void testListener () {
        Debug.Log ("xxxx");
    }

    private void newTestListener (object sender, EventArgs generalEventArgs) { }

    private void loadCubeCallback () {
        assetManager.GetAssetByUrlAsync<GameObject> ("Shape/Cube", (res) => {
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

        assetManager.TryReleaseAsset ("Shape/Cube");
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
        App.Make<IAssetsManager> ().GetAssetByBundleAsync<GameObject> ("resbundle", "Cube", (GameObject cubeAsset) => {
            GameObject cube = Instantiate<GameObject> (cubeAsset);
            cube.transform.SetParent (nodeParent);
            cube.transform.localPosition = Vector3.zero;
        });

        App.Make<IAssetsManager> ().GetAssetByBundleAsync<GameObject> ("resbundle", "Cube", (GameObject cubeAsset) => {
            GameObject cube = Instantiate<GameObject> (cubeAsset);
            cube.transform.SetParent (nodeParent);
            cube.transform.localPosition = Vector3.zero;
        });

        test ();

        // 同步加载文件夹资源
        // List<PackAsset> assetList = AssetsManager.instance.getAllAssetsByUrlSync<Sprite> ("Textures/");
        // Sprite attackSprite = AssetsManager.instance.getAssetByUrlSync<Sprite> ("Textures/attack");

        // 使用bundle同步加载文件夹资源
        // List<PackAsset> assetList = AssetsManager.instance.getAllAssetsByBundleSync<Sprite> ("texturesbundle");
        // Sprite attackSprite = AssetsManager.instance.getAssetByBundleSync<Sprite> ("texturesbundle", "attack");

        // 使用bundle异步加载文件夹资源
        App.Make<IAssetsManager> ().GetAllAssetsByBundleASync<Sprite> ("texturesbundle", (List<PackAsset> assetsArray) => {
            List<PackAsset> assets = assetsArray;
            Sprite attackSprite = App.Make<IAssetsManager> ().GetAssetByBundleSync<Sprite> ("texturesbundle", "attack");
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
            .Then (() => {
                Debug.Log ("over");
            });
    }
}

public class InjectTest {
    public int intValue;

    public string stringValue;

    private int priIntValue;

    private string priStrValue;

    public int intProperty {
        get {
            return 1;
        }
        set {
            intValue = value;
        }
    }

    private string strProperty {
        get {
            return "1";
        }
        set {
            stringValue = value;
        }
    }
}