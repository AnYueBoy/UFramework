using UnityEngine;
namespace UFramework.GameCommon {

    public interface IAudioManager {

        void init (GameObject attach);

        void playSound (string soundName);

        void playMusic (string musicName, bool isLoop = true);

        void pauseMusic ();

        void stopMusic ();

        void resumeMusic ();
    }
}