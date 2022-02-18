/*
 * @Author: l hy 
 * @Date: 2020-12-21 16:27:14 
 * @Description: 音效管理
 */

using System.Collections.Generic;
using UFramework.FrameUtil;
using UnityEngine;
namespace UFramework.GameCommon {

    public class AudioManager : IAudioManager {

        private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip> ();

        private AudioSource audioSource = null;

        public void init (GameObject attach) {
            this.audioSource = attach.AddComponent<AudioSource> ();
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundName">音效名</param>
        public void playSound (string soundName) {

            AudioClip targetClip = this.getAudioClipByName (soundName);

            this.audioSource.PlayOneShot (targetClip);
        }

        public void playMusic (string musicName, bool isLoop = true) {
            if (!CommonUtil.isVialid (musicName)) {
                return;
            }

            if (this.audioSource.clip) {
                if (this.audioSource.clip.name == musicName) {
                    return;
                }
            }

            AudioClip targetClip = this.getAudioClipByName (musicName);
            this.audioSource.clip = targetClip;
            this.audioSource.loop = isLoop;
            this.audioSource.Play ();
        }

        public void pauseMusic () {
            if (!this.audioSource) {
                return;
            }

            if (!this.audioSource.clip) {
                return;
            }

            if (!this.audioSource.isPlaying) {
                return;
            }

            this.audioSource.Pause ();
        }

        public void stopMusic () {
            if (!this.audioSource) {
                return;
            }

            if (!this.audioSource.clip) {
                return;
            }

            if (!this.audioSource.isPlaying) {
                return;
            }

            this.audioSource.Stop ();
            this.audioSource.clip = null;
        }

        public void resumeMusic () {
            if (!this.audioSource) {
                return;
            }

            if (!this.audioSource.clip) {
                return;
            }

            if (this.audioSource.isPlaying) {
                return;
            }

            this.audioSource.Play ();
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