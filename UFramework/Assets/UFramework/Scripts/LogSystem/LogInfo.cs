/*
 * @Author: l hy 
 * @Date: 2021-01-25 15:38:28 
 * @Description: log信息
 */

namespace UFramework.LogSystem {
    using UnityEngine;
    public class LogInfo {
        public string logContent;
        public string logTrack;
        public LogType logType;

        public LogInfo (string logContent, string logTrack, LogType logType) {
            this.logContent = logContent;
            this.logTrack = logTrack;
            this.logType = logType;
        }
    }
}