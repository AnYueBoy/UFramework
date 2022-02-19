/*
 * @Author: l hy 
 * @Date: 2021-01-25 15:45:33 
 * @Description: 日志输出线程
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace UFramework.LogSystem {

    public class LogOutThread {

#if UNITY_EDITOR
        string mDevicePersistentPath = Application.dataPath + "/../";
#elif UNITY_STANDALONE_WIN
        string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
#elif UNITY_STANDALONE_OSX  
        string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
#else
        string mDevicePersistentPath = Application.persistentDataPath;
#endif 

        static string postFixLogPath = ".log";
        private Queue<LogInfo> writingLogQueue = null;
        private Queue<LogInfo> waitingLogQueue = null;
        private object logLock = null;
        private Thread fileLogThread = null;
        private bool isRunning = false;
        private StreamWriter logWriter = null;

        public void Init () {
            this.writingLogQueue = new Queue<LogInfo> ();
            this.waitingLogQueue = new Queue<LogInfo> ();
            this.logLock = new object ();

            DateTime currentTime = DateTime.Now;
            // 以时间戳作为log文件的文件名称
            string logName = string.Format ("Log{0}{1}{2}#{3}{4}{5}",
                currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);

            // log文件完整路径
            string logPath = string.Format ("{0}/{1}/{2}.txt", mDevicePersistentPath, postFixLogPath, logName);

            if (File.Exists (logPath)) {
                File.Delete (logPath);
            }

            // log文件所在文件夹
            string logDir = Path.GetDirectoryName (logPath);
            if (!Directory.Exists (logDir)) {
                Directory.CreateDirectory (logDir);
            }

            // 创建写入流
            this.logWriter = new StreamWriter (logPath);
            this.logWriter.AutoFlush = true;
            this.isRunning = true;
            this.fileLogThread = new Thread (new ThreadStart (this.WriteLog));
            this.fileLogThread.Start ();

            Debug.Log (logPath);
        }

        private void WriteLog () {
            while (this.isRunning) {
                if (this.writingLogQueue.Count == 0) {
                    lock (this.logLock) {
                        while (this.waitingLogQueue.Count == 0) {
                            Monitor.Wait (this.logLock);
                        }
                        Queue<LogInfo> tmpQueue = this.writingLogQueue;
                        this.writingLogQueue = this.waitingLogQueue;
                        this.waitingLogQueue = tmpQueue;
                    }
                } else {
                    while (this.writingLogQueue.Count > 0) {
                        LogInfo logInfo = this.writingLogQueue.Dequeue ();
                        // 严重级别log，打印调用栈
                        if (logInfo.logType == LogType.Error ||
                            logInfo.logType == LogType.Exception ||
                            logInfo.logType == LogType.Assert) {
                            this.logWriter.WriteLine ("---------------------------");
                            this.logWriter.WriteLine (DateTime.Now.ToString () + "\t" + logInfo.logContent + "\n");
                            this.logWriter.WriteLine (logInfo.logTrack);
                            this.logWriter.WriteLine ("---------------------------");
                        } else {
                            this.logWriter.WriteLine (DateTime.Now.ToString () + "\t" + logInfo.logContent);
                        }
                    }
                }
            }
        }

        public void Log (LogInfo logData) {
            lock (this.logLock) {
                this.waitingLogQueue.Enqueue (logData);
                Monitor.Pulse (this.logLock);
            }
        }

        private void Close () {
            this.isRunning = false;
            this.logWriter.Close ();
        }

        public void Quit () {
            this.Close ();
        }
    }
}