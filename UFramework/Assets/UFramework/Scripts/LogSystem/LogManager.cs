/*
 * @Author: l hy 
 * @Date: 2021-01-21 21:47:35 
 * @Description: 调试信息管理
 * @Last Modified by: l hy
 * @Last Modified time: 2021-01-25 17:28:21
 */

using UFramework.Core;
using UnityEngine;
namespace UFramework.LogSystem {
    public class LogManager : ILogManager {
        private LogOutThread logOutThread = new LogOutThread ();

        public void Init () {
            if (App.DebugLevel != DebugLevel.Development) {
                return;
            }
            logOutThread.Init ();

            UnityEngine.Application.logMessageReceived += UnityLogCallback;
            UnityEngine.Application.logMessageReceivedThreaded += UnityLogCallbackThread;
        }

        private void UnityLogCallbackThread (string log, string track, LogType logType) {
            LogInfo logInfo = new LogInfo (log, track, logType);
            logOutThread.Log (logInfo);
        }

        private void UnityLogCallback (string log, string track, LogType logType) {
            LogInfo logInfo = new LogInfo (log, track, logType);
        }

        public void Quit () {
            logOutThread.Quit ();
        }
    }
}