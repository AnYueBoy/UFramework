﻿using System;
using System.Collections.Generic;
using System.Reflection;
/*
 * @Author: l hy 
 * @Date: 2021-01-21 22:15:59 
 * @Description: 用于各类测试项目
 * @Last Modified by: l hy
 * @Last Modified time: 2022-01-14 14:22:29
 */

using System.Threading.Tasks;
using UFramework;
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

        ModuleManager.instance.promiseTimer.waitFor (3).then (
            () => {
                Debug.Log ("时间等待结束");
            }
        );

        ListenerManager.getInstance ().add ("event1", this, this.testListener);
        ListenerManager.getInstance ().add ("event2", this, this.testListener);
        eventDispatcher.AddListener ("eventDispatcher", newTestListener);

        InjectTest injectTest = new InjectTest ();

        foreach (var property in injectTest.GetType ().GetProperties (BindingFlags.Public | BindingFlags.Instance)) {
            Debug.Log ($"{property.PropertyType}");
            Debug.Log("");
        }

    }

    private void testListener () {
        Debug.Log ("xxxx");
    }

    private void newTestListener (object sender, EventArgs generalEventArgs) {
        GeneralEventArgs args = (GeneralEventArgs) generalEventArgs;
        Debug.Log ($"what fuck {args.Data}");
    }

    public void event1 () {
        ListenerManager.getInstance ().trigger ("event1");
        eventDispatcher.Raise ("eventDispatcher", new GeneralEventArgs ("test"));
        eventDispatcher.RemoveListener ("eventDispatcher", newTestListener);
    }

    public void event2 () {
        ListenerManager.getInstance ().trigger ("event2");
    }

    public void unloadAll () {
        ListenerManager.getInstance ().removeAll (this);
    }

    public void unloadAt () {
        ListenerManager.getInstance ().removeAt ("event1", this);
    }

    private async void loadCube () {
        // 不允许在非主线程中调用unity的api
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
        AssetsManager.instance.getAssetByBundleAsync<GameObject> ("resbundle", "Cube", (GameObject cubeAsset) => {
            GameObject cube = Instantiate<GameObject> (cubeAsset);
            cube.transform.SetParent (nodeParent);
            cube.transform.localPosition = Vector3.zero;
        });

        AssetsManager.instance.getAssetByBundleAsync<GameObject> ("resbundle", "Cube", (GameObject cubeAsset) => {
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
        AssetsManager.instance.getAllAssetsByBundleASync<Sprite> ("texturesbundle", (List<PackAsset> assetsArray) => {
            List<PackAsset> assets = assetsArray;
            Sprite attackSprite = AssetsManager.instance.getAssetByBundleSync<Sprite> ("texturesbundle", "attack");
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