/*
 * @Author: l hy 
 * @Date: 2020-04-10 21:54:42 
 * @Description: 场景管理器 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:43:20
 */
namespace UFramework.GameCommon {

    using System.Linq;
    using System;
    using UnityEngine.SceneManagement;
    using UnityEngine;
    using UFramework.ConfigData;

    public class SceneLoadManager {

        private static SceneLoadManager instance = null;

        public static SceneLoadManager GetInstance () {
            if (instance == null) {
                instance = new SceneLoadManager ();
            }

            return instance;
        }

        /// <summary>
        /// 加载下一场景
        /// </summary>
        /// <param name="isFromSceneConfig">是否从场景配置文件中加载场景</param>
        public void LoadNextScene (bool isFromSceneConfig = false, Action<AsyncOperation> callBack = null) {
            if (!isFromSceneConfig) {
                int currentSceneIndex = SceneManager.GetActiveScene ().buildIndex;
                int resultSceneIndex = currentSceneIndex + 1;
                int allSceneCount = SceneManager.sceneCountInBuildSettings;
                if (resultSceneIndex > allSceneCount) {
                    resultSceneIndex = allSceneCount;
                }

                this.LoadScene (resultSceneIndex, callBack);
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

            this.LoadScene (nextSceneName, callBack);
        }

        public void LoadAppointScene (string sceneName, Action<AsyncOperation> callBack = null) {
            if (string.IsNullOrEmpty (sceneName)) {
                return;
            }

            this.LoadScene (sceneName, callBack);
        }

        public void LoadScene (string sceneName, Action<AsyncOperation> callBack = null) {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync (sceneName);

            if (callBack != null) {
                asyncOperation.completed += callBack;
            }
        }

        public void LoadScene (int sceneIndex, Action<AsyncOperation> callBack = null) {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync (sceneIndex);

            if (callBack != null) {
                asyncOperation.completed += callBack;
            }
        }
    }
}