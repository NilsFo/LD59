using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UILogger : MonoBehaviour
{
    private string myLog;
    private List<LogObject> logList;
    private string _outPath;

    private float fpsUpdateTimerCurrent = 0;
    private float fpsCache = 0;
    public float pfsUpdateTimer = 1;

    public TextMeshProUGUI text;
    public bool showFPS;
    public bool showLog;
    public bool showWholeStackTrace = true;
    public bool writeLogToFile = false;

    void Start()
    {
        _outPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        logList = new List<LogObject>();
        fpsUpdateTimerCurrent = 0;
        fpsCache = 0;
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        fpsUpdateTimerCurrent = 0;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        fpsUpdateTimerCurrent = 0;
    }

    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        if (showWholeStackTrace)
        {
            newString = newString + "\n" + stackTrace;
        }

        if (writeLogToFile)
        {
            string fileName = "Log_" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string filePath = _outPath + "/" + fileName;
            File.AppendAllLines(filePath, new[] { newString });
        }

        if (logList != null && showLog)
        {
            logList.Add(new LogObject(newString));
        }
    }

    private void Update()
    {
        fpsUpdateTimerCurrent = fpsUpdateTimerCurrent + Time.deltaTime;
        if (fpsUpdateTimerCurrent > pfsUpdateTimer)
        {
            fpsUpdateTimerCurrent = fpsUpdateTimerCurrent - pfsUpdateTimer;
            fpsCache = Mathf.Floor(1.0f / Time.deltaTime);
        }
    }

    private void OnGUI()
    {
        string logText = "";
        if (showFPS)
        {
            logText = "FPS: " + (int)fpsCache;
        }

        float dt = Time.deltaTime;
        if (logList == null)
        {
            logList = new List<LogObject>();
            return;
        }

        ArrayList deleteList = new ArrayList();
        foreach (LogObject logObject in logList)
        {
            logObject.Tick(dt);
            if (logObject.IsExpired())
            {
                deleteList.Add(logObject);
            }
            else
            {
                if (showLog)
                {
                    logText = logText + "\n" + logObject.GetText();
                    logText = logText.Trim();
                }
            }
        }

        foreach (LogObject logObject in deleteList)
        {
            logList.Remove(logObject);
        }

        text.text = logText;
    }

    public class LogObject
    {
        private string text;
        private float timeout;

        public LogObject(string text)
        {
            this.text = text;
            this.timeout = 5.69f;
        }

        public string GetText()
        {
            return text;
        }

        public void Tick(float time)
        {
            timeout -= time;
        }

        public bool IsExpired()
        {
            return timeout < 0;
        }

        public float GetTimeout()
        {
            return timeout;
        }
    }
}