using UnityEngine;
public class LogManager : ILogManager {

    public LogManager (string value) {
        Debug.Log ($"Log Manager construct value {value}");
    }
    public void printLog () {
        Debug.Log ("Log Manager Test");
    }
}