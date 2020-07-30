using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    private static AudioManager instance = null;

    public static AudioManager getInstance () {
        return instance;
    }

    private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip> ();

    private AudioSource audioSource = null;

    private void Awake () {
        instance = this;

        gameObject.AddComponent<AudioListener> ();
        this.audioSource = gameObject.AddComponent<AudioSource> ();
    }

    public void playSoundByName (string soundName, bool isOnShot = false) {
        if (!this.audioSource) {
            return;
        }

        if (!this.clipDic.ContainsKey (soundName)) {
            AudioClip clip = this.loadClipByUrl (soundName);
            if (clip == null) {
                return;
            }

            this.clipDic.Add (soundName, clip);
        }

        AudioClip targetClip = this.clipDic[soundName];

        if (isOnShot) {
            this.audioSource.PlayOneShot (targetClip);
        } else {
            this.audioSource.clip = targetClip;
            this.audioSource.Play ();
        }

    }

    private AudioClip loadClipByUrl (string url) {
        AudioClip clip = Resources.Load<AudioClip> (url);
        return clip;
    }

}