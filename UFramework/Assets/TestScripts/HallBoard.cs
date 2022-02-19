using System;
using System.Collections;
using System.Collections.Generic;
using UFramework.Core;
using UFramework.GameCommon;
using UnityEngine;

public class HallBoard : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        App.Make<IAudioManager> ().PlaySound ("test");
    }

    // Update is called once per frame
    void Update () {

    }

    public void loadNextScene () {
        SceneLoadManager.GetInstance ().LoadNextScene (false, (AsyncOperation asy) => { Debug.Log ("加载完成"); });
    }

    public void playBGM () {
        App.Make<IAudioManager> ().PlayMusic ("BGM");
    }

    public void pauseBGM () {
        App.Make<IAudioManager> ().PauseMusic ();
    }

    public void stopBGM () {
        App.Make<IAudioManager> ().StopMusic ();
    }

    public void resumeBGM () {
        App.Make<IAudioManager> ().ResumeMusic ();
    }
}