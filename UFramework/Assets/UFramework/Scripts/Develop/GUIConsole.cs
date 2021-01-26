/*
 * @Author: l hy 
 * @Date: 2021-01-25 18:58:53 
 * @Description: 控制台
 */
namespace UFrameWork.Develop {
    using System.Collections.Generic;
    using UFrameWork.Application;
    using UnityEngine;
    public class GUIConsole {

        public delegate void onUpdateCallback ();
        public delegate void onGUICallback ();

        public static onUpdateCallback updateCallback = null;
        public static onGUICallback gUICallback = null;

        private static FPSCounter fPSCounter = null;
        private static MemoryDetector memoryDetector = null;
        private static bool showGUI = false;
        private static List<ConsoleMessage> entries = new List<ConsoleMessage> ();
        private static Vector2 scrollPos;
        private static bool scrollToBottom = true;
        private static bool collapse;
        private static bool touching = false;

        private const int margin = 20;
        private static Rect windowRect = new Rect (margin + Screen.width * 0.5f, margin, Screen.width * 0.5f - (2 * margin), Screen.height - (2 * margin));

        private static GUIContent clearLabel = new GUIContent ("Clear", "Clear the contents of the console.");
        private static GUIContent collapseLabel = new GUIContent ("Collapse", "Hide repeated messages.");
        private static GUIContent scrollToBottomLabel = new GUIContent ("ScrollToBottom", "Scroll bar always at bottom");

        public static void init () {
            fPSCounter = new FPSCounter ();
            memoryDetector = new MemoryDetector ();

            ApplicationManager.applicationUpdate += Update;
            ApplicationManager.applicationOnGUI += OnGUI;
            Application.logMessageReceived += handleLog;
        }

        private static void Update () {
#if UNITY_EDITOR
            if (Input.GetKey (KeyCode.F1)) {
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
            if (updateCallback != null) {
                updateCallback ();
            }

        }

        private static void OnGUI () {
            if (!showGUI) {
                return;
            }

            if (gUICallback != null) {
                gUICallback ();
            }

            windowRect = GUILayout.Window (123456, windowRect, consoleWindow, "Console");
        }

        private static void consoleWindow (int windowID) {
            if (scrollToBottom) {
                GUILayout.BeginScrollView (Vector2.up * entries.Count * 100.0f);
            } else {
                scrollPos = GUILayout.BeginScrollView (scrollPos);
            }

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

                if (entry.type == LogType.Exception) {
                    GUILayout.Label (entry.message + "||" + entry.stackTrace);
                } else {
                    GUILayout.Label (entry.message);
                }

                GUI.contentColor = Color.white;
                GUILayout.EndScrollView ();

                GUILayout.BeginHorizontal ();
                if (GUILayout.Button (clearLabel)) {
                    entries.Clear ();
                }

                collapse = GUILayout.Toggle (collapse, collapseLabel, GUILayout.ExpandWidth (false));
                scrollToBottom = GUILayout.Toggle (scrollToBottom, scrollToBottomLabel, GUILayout.ExpandWidth (false));
                GUILayout.EndHorizontal ();

                GUI.DragWindow (new Rect (0, 0, 10000, 20));
            }
        }

        private static void handleLog (string message, string stackTrace, LogType type) {
            ConsoleMessage entry = new ConsoleMessage (message, stackTrace, type);
            entries.Add (entry);
        }

        ~GUIConsole () {
            Application.logMessageReceived -= handleLog;
        }
    }
}