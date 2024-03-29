using UnityEngine;
namespace UFramework {

    public interface IAudioManager {

        void Init (GameObject attach);

        void PlaySound (string soundName);

        void PlayMusic (string musicName, bool isLoop = true);

        void PauseMusic ();

        void StopMusic ();

        void ResumeMusic ();
    }
}