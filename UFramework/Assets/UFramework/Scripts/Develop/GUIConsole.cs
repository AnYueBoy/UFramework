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