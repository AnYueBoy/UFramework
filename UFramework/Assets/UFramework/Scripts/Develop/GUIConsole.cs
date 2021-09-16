/*
 * @Author: l hy 
 * @Date: 2021-01-25 18:58:53 
 * @Description: 控制台
 */
namespace UFramework.Develop {
    using System.Collections.Generic;
    using UFramework.LogSystem;
    using UnityEngine;
    public class GUIConsole {

        #region  信息组件
        /*帧率计算 */
        private FPSCounter fPSCounter = null;

        /*内存监视*/
        private MemoryDetector memoryDetector = null;

        /* log管理 */
        private LogManager logManager = null;
        #endregion

        private bool showGUI = false;
        private List<ConsoleMessage> entries = new List<ConsoleMessage> ();
        private bool touching = false;
        public void init () {
            fPSCounter = new FPSCounter ();
            memoryDetector = new MemoryDetector ();
            logManager = new LogManager ();

            this.fPSCounter.init ();
            memoryDetector.init ();
            logManager.init ();

            Application.logMessageReceivedThreaded += handleLog;
        }

        public void localUpdate (float dt) {
#if UNITY_EDITOR
            if (Input.GetKeyUp (KeyCode.F1)) {
                showGUI = !showGUI;
            }

#elif UNITY_ANDROID ||UNITY_IOS
            if (!touching && Input.touchCount >= 3) {
                touching = true;
                showGUI = !showGUI;
            } else if (Input.touchCount == 0) {
                touching = false;
            }
#endif

            this.fPSCounter.localUpdate ();
        }

        private const int margin = 20;
        private Rect windowRect = new Rect (Screen.width / 2, margin, Screen.width / 2 - margin, Screen.height - margin);

        public void drawGUI () {
            if (!showGUI) {
                return;
            }

            this.fPSCounter.drawGUI ();
            this.memoryDetector.drawGUI ();

            windowRect = GUILayout.Window (100, windowRect, consoleWindow, "Console");
        }

        private readonly GUIContent clearLabel = new GUIContent ("Clear", "Clear the contents of the console.");
        private readonly GUIContent collapseLabel = new GUIContent ("Collapse", "Hide repeated messages.");
        private readonly GUIContent scrollToBottomLabel = new GUIContent ("ScrollToBottom", "Scroll bar always at bottom");
        private bool collapse;
        private Vector2 scrollPos;

        private void consoleWindow (int windowID) {
            scrollPos = GUILayout.BeginScrollView (scrollPos);

            for (int i = 0; i < entries.Count; i++) {
                ConsoleMessage entry = entries[i];
                if (collapse && i > 0 && entry.message == entries[i - 1].message) {
                    continue;
                }

                switch (entry.type) {
                    case LogType.Error:
                    case LogType.Exception:
                        GUI.contentColor = Color.red;
                        break;
                    case LogType.Warning:
                        GUI.contentColor = Color.yellow;
                        break;
                    default:
                        GUI.contentColor = Color.white;
                        break;
                }

                if (entry.type == LogType.Exception || entry.type == LogType.Error) {
                    GUILayout.Label (entry.message + " || " + entry.stackTrace);
                } else {
                    GUILayout.Label (entry.message);
                }
            }

            GUILayout.EndScrollView ();

            GUILayout.BeginHorizontal ();
            if (GUILayout.Button (clearLabel)) {
                entries.Clear ();
            }

            collapse = GUILayout.Toggle (collapse, collapseLabel, GUILayout.ExpandWidth (false));
            GUILayout.EndHorizontal ();

            GUI.DragWindow (new Rect (0, 0, 10000, 20));
        }

        private void handleLog (string message, string stackTrace, LogType type) {
            ConsoleMessage entry = new ConsoleMessage (message, stackTrace, type);
            entries.Add (entry);
        }

        public void quit () {
            Application.logMessageReceivedThreaded -= handleLog;
            this.logManager.quit ();
        }
    }
}