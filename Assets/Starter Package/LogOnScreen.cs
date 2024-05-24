using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
     
public class LogOnScreen : UnitySingleton<LogOnScreen>{
     
    public uint qsize = 15;  // number of messages to keep
    public int fontSize = 40;
    Queue myLogQueue = new Queue();
     
    void Start() {
        Debug.Log("Started up logging.");
    }
     
    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }
     
    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }
     
    public void HandleLog(string logString, string stackTrace, LogType type) {
        myLogQueue.Enqueue("[" + type + "] " + logString);
        if(type == LogType.Exception) myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }
     
    public void OnGUI() {
        GUI.skin.label.fontSize = fontSize;
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();
    }
}