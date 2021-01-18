/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:27:14 
 * @Description: 音效管理
 */

namespace UFramework.GameCommon {

    using System.Collections.Generic;
    using UFramework.FrameUtil;
    using UnityEngine;
    public class AudioManager : MonoBehaviour {
        private static AudioManager instance = null;

        public static AudioManager getInstance () {
            return instance;
        }

        private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip> ();

        private void Awake () {
            instance = this;

            gameObject.AddComponent<AudioListener> ();
        }

        private AudioSource soundSource = null;

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundName">音效名</param>
        public void playSound (string soundName) {
            if (!this.soundSource) {
                this.soundSource = gameObject.AddComponent<AudioSource> ();
            }

            AudioClip targetClip = this.getAudioClipByName (soundName);

            this.soundSource.PlayOneShot (targetClip);
        }

        private AudioSource musicSource = null;

        public void playMusic (string musicName, bool isLoop = true) {
            if (!CommonUtil.isVialid (musicName)) {
                return;
            }

            if (!this.musicSource) {
                this.musicSource = this.gameObject.AddComponent<AudioSource> ();
            }

            if (this.musicSource.clip) {
                if (this.musicSource.clip.name == musicName) {
                    return;
                }
            }

            AudioClip targetClip = this.getAudioClipByName (musicName);
            this.musicSource.clip = targetClip;
            this.musicSource.loop = isLoop;
            this.musicSource.Play ();
        }

        public void pauseMusic () {
            if (!this.musicSource) {
                return;
            }

            if (!this.musicSource.clip) {
                return;
            }

            if (!this.musicSource.isPlaying) {
                return;
            }

            this.musicSource.Pause ();
        }

        public void stopMusic () {
            if (!this.musicSource) {
                return;
            }

            if (!this.musicSource.clip) {
                return;
            }

            if (!this.musicSource.isPlaying) {
                return;
            }

            this.musicSource.Stop ();
            this.musicSource.clip = null;
        }

        public void resumeMusic () {
            if (!this.musicSource) {
                return;
            }

            if (!this.musicSource.clip) {
                return;
            }

            if (this.musicSource.isPlaying) {
                return;
            }

            this.musicSource.Play ();
        }

        private AudioClip getAudioClipByName (string clipName) {
            if (this.clipDic.ContainsKey (clipName)) {
                return this.clipDic[clipName];
            }

            AudioClip clip = this.loadClipByUrl (clipName);
            if (clip == null) {
                return null;
            }

            this.clipDic.Add (clipName, clip);
            return clip;
        }

        private AudioClip loadClipByUrl (string url) {
            AudioClip clip = Resources.Load<AudioClip> (url);
            return clip;
        }
    }
}