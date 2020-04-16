using System.Linq;
/*
 * @Author: l hy 
 * @Date: 2020-04-10 21:54:42 
 * @Description: 场景管理器 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-04-14 21:50:19
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
    public void loadNextScene (bool isSceneIndex = true) {
        if (isSceneIndex) {
            int currentSceneIndex = SceneManager.GetActiveScene ().buildIndex;
            int resultSceneIndex = currentSceneIndex + 1;
            int allSceneCount = SceneManager.sceneCountInBuildSettings;
            if (resultSceneIndex > allSceneCount) {
                resultSceneIndex = allSceneCount;
            }

            SceneManager.LoadScene (resultSceneIndex);
            return;
        }

        string nextSceneName = null;
        string currentSceneName = SceneManager.GetActiveScene ().name;
        for (int i = 0; i < SceneConfig.sceneConfigs.Count (); i++) {
            string sceneName = SceneConfig.sceneConfigs[i];
            if (string.IsNullOrEmpty (sceneName)) {
                continue;
            }

            if (currentSceneName == sceneName) {
                if (i >= SceneConfig.sceneConfigs.Count ()) {
                    break;
                }

                nextSceneName = SceneConfig.sceneConfigs[i + 1];
                break;
            }
        }

        if (string.IsNullOrEmpty (nextSceneName)) {
            Debug.LogError ("load nextSceneName is null");
            return;
        }

        SceneManager.LoadScene (nextSceneName);
    }

    public void loadAppointScene (string sceneName) {
        if (string.IsNullOrEmpty (sceneName)) {
            return;
        }

        SceneManager.LoadScene (sceneName);
    }
}