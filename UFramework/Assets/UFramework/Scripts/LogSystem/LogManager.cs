/*
 * @Author: l hy 
 * @Date: 2021-01-21 21:47:35 
 * @Description: 调试信息管理
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-25 17:28:21
 */

namespace UFramework.LogSystem {
    using UnityEngine;
    public class LogManager {
        private LogOutThread logOutThread = new LogOutThread ();

        public void init () {
            logOutThread.init ();

            Application.logMessageReceived += unityLogCallback;
            Application.logMessageReceivedThreaded += unityLogCallbackThread;
        }

        private void unityLogCallbackThread (string log, string track, LogType logType) {
            LogInfo logInfo = new LogInfo (log, track, logType);
            logOutThread.log (logInfo);
        }

        private void unityLogCallback (string log, string track, LogType logType) {
            LogInfo logInfo = new LogInfo (log, track, logType);
        }

        public void quit () {
            logOutThread.quit ();
        }
    }
}