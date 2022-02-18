using System;
using System.Collections;
using System.Collections.Generic;
using UFramework.Core;
using UFramework.GameCommon;
using UnityEngine;

public class HallBoard : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        App.Make<IAudioManager> ().playSound ("test");
    }

    // Update is called once per frame
    void Update () {

    }

    public void loadNextScene () {
        SceneLoadManager.getInstance ().loadNextScene (false, (AsyncOperation asy) => { Debug.Log ("加载完成"); });
    }

    public void playBGM () {
        App.Make<IAudioManager> ().playMusic ("BGM");
    }

    public void pauseBGM () {
        App.Make<IAudioManager> ().pauseMusic ();
    }

    public void stopBGM () {
        App.Make<IAudioManager> ().stopMusic ();
    }

    public void resumeBGM () {
        App.Make<IAudioManager> ().resumeMusic ();
    }
}