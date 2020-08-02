using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallBoard : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        AudioManager.getInstance ().playSound ("test");
    }

    // Update is called once per frame
    void Update () {

    }

    public void loadNextScene () {
        SceneLoadManager.getInstance ().loadNextScene (false, (AsyncOperation asy) => { Debug.Log ("加载完成"); });
    }

    public void playBGM () {
        AudioManager.getInstance ().playMusic ("BGM");
    }

    public void pauseBGM () {
        AudioManager.getInstance ().pauseMusic ();
    }

    public void stopBGM () {
        AudioManager.getInstance ().stopMusic ();
    }

     public void resumeBGM () {
        AudioManager.getInstance ().resumeMusic ();
    }
}