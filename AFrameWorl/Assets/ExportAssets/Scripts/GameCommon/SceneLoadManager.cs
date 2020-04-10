/*
 * @Author: l hy 
 * @Date: 2020-04-10 21:54:42 
 * @Description: 场景管理器 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-04-10 22:34:12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager {

    private static SceneLoadManager instance = null;

    public static SceneLoadManager getInstance () {
        if (instance == null) {
            instance = new SceneLoadManager ();
        }

        return instance;
    }

    /// <summary>
    /// 切换下一场景
    /// </summary>
    public void loadNextScene () {
        int currentSceneIndex = SceneManager.GetActiveScene ().buildIndex;
        int resultSceneIndex = currentSceneIndex + 1;
        int allSceneCount = SceneManager.sceneCountInBuildSettings;
        if (resultSceneIndex > allSceneCount) {
            resultSceneIndex = allSceneCount;
        }

        SceneManager.LoadScene (resultSceneIndex);
    }

    public void loadAppointScene (string sceneName) {
        if (sceneName == "" || sceneName == null) {
            return;
        }

        SceneManager.LoadScene (sceneName);
    }
}