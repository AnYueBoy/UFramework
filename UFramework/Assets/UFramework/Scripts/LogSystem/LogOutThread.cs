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
using SApplication = UnityEngine.Application;

namespace UFramework
{
    public class LogOutThread
    {
#if UNITY_EDITOR
        string mDevicePersistentPath = SApplication.dataPath + "/../";
#elif UNITY_STANDALONE_WIN
        string mDevicePersistentPath = SApplication.dataPath + "/PersistentPath";
#elif UNITY_STANDALONE_OSX
        string mDevicePersistentPath = SApplication.dataPath + "/PersistentPath";
#else
        string mDevicePersistentPath = SApplication.persistentDataPath;
#endif

        static string postFixLogPath = ".log";
        private Queue<LogInfo> writingLogQueue = null;
        private Queue<LogInfo> waitingLogQueue = null;
        private object logLock = null;
        private Thread fileLogThread = null;
        private bool isRunning = false;
        private StreamWriter logWriter = null;

        public void Init()
        {
            writingLogQueue = new Queue<LogInfo>();
            waitingLogQueue = new Queue<LogInfo>();
            logLock = new object();

            DateTime currentTime = DateTime.Now;
            // 以时间戳作为log文件的文件名称
            string logName = string.Format("Log{0}{1}{2}#{3}{4}{5}",
                currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute,
                currentTime.Second);

            // log文件完整路径
            string logPath = string.Format("{0}/{1}/{2}.txt", mDevicePersistentPath, postFixLogPath, logName);

            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }

            // log文件所在文件夹
            string logDir = Path.GetDirectoryName(logPath);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            // 创建写入流
            logWriter = new StreamWriter(logPath);
            logWriter.AutoFlush = true;
            isRunning = true;
            fileLogThread = new Thread(new ThreadStart(WriteLog));
            fileLogThread.Start();

            Debug.Log(logPath);
        }

        private void WriteLog()
        {
            while (isRunning)
            {
                if (writingLogQueue.Count == 0)
                {
                    lock (logLock)
                    {
                        while (waitingLogQueue.Count == 0)
                        {
                            Monitor.Wait(logLock);
                        }

                        Queue<LogInfo> tmpQueue = writingLogQueue;
                        writingLogQueue = waitingLogQueue;
                        waitingLogQueue = tmpQueue;
                    }
                }
                else
                {
                    while (writingLogQueue.Count > 0)
                    {
                        LogInfo logInfo = writingLogQueue.Dequeue();
                        // 严重级别log，打印调用栈
                        if (logInfo.logType == LogType.Error ||
                            logInfo.logType == LogType.Exception ||
                            logInfo.logType == LogType.Assert)
                        {
                            logWriter.WriteLine("---------------------------");
                            logWriter.WriteLine(DateTime.Now.ToString() + "\t" + logInfo.logContent + "\n");
                            logWriter.WriteLine(logInfo.logTrack);
                            logWriter.WriteLine("---------------------------");
                        }
                        else
                        {
                            logWriter.WriteLine(DateTime.Now.ToString() + "\t" + logInfo.logContent);
                        }
                    }
                }
            }
        }

        public void Log(LogInfo logData)
        {
            lock (logLock)
            {
                waitingLogQueue.Enqueue(logData);
                Monitor.Pulse(logLock);
            }
        }

        private void Close()
        {
            isRunning = false;
            logWriter.Close();
        }

        public void Quit()
        {
            Close();
        }
    }
}