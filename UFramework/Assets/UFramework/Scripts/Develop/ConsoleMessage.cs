/*
 * @Author: l hy 
 * @Date: 2021-01-26 09:22:22 
 * @Description: 控制台信息
 */

namespace UFramework.Develop {
    using UnityEngine;
    public class ConsoleMessage {

        public readonly string message;
        public readonly string stackTrace;
        public readonly LogType type;

        public ConsoleMessage (string message, string stackTrace, LogType type) {
            this.message = message;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }
}