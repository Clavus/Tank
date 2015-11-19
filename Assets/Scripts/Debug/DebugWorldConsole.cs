using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Canvas))]
public class DebugWorldConsole : MonoBehaviour
{
    private static DebugWorldConsole main;

    private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
    {
        {LogType.Assert, Color.white},
        {LogType.Error, Color.red},
        {LogType.Exception, Color.red},
        {LogType.Log, Color.white},
        {LogType.Warning, Color.yellow}
    };

    private readonly List<Log> logs = new List<Log>();

    public bool collapse = true;
    private bool logWasUpdated;
    public int numberOfLines = 22;
    public bool show = true;
    public Text textElement;

    /// <summary>
    ///     The hotkey to show and hide the console window.
    /// </summary>
    public KeyCode toggleKey = KeyCode.BackQuote;

    private void Awake()
    {
        if (main != null) return;
        //Debug.Log("World console set");
        main = this;
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        UpdateText();
        //Debug.Log("World console enabled!");
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            show = !show;
            UpdateText();
        }

        if (logWasUpdated)
        {
            UpdateText();
            logWasUpdated = false;
        }
    }

    private void UpdateText()
    {
        textElement.text = "";
        if (!show)
            return;

        int lines = 0;
        string text = "";

        // Iterate through the recorded logs, backwards
        for (var i = logs.Count - 1; i >= 0; i--)
        {
            Log log = logs[i];
            if (log.message == "")
                continue;

            // Combine identical messages
            if (collapse && i < logs.Count - 1)
            {
                if (log.message == logs[i + 1].message)
                    continue;
            }

            Color col = logTypeColors[log.type];
            string hex = string.Format("#{0}{1}{2}",
                ((int) (col.r*255)).ToString("X2"),
                ((int) (col.g*255)).ToString("X2"),
                ((int) (col.b*255)).ToString("X2"));

            text = "<color=#" + hex + ">" + log.message + "</color>" + Environment.NewLine + text;

            lines++;
            if (lines >= numberOfLines)
                break;
        }

        textElement.text = text;
    }

    private void HandleLog(string message, string stackTrace, LogType type)
    {
        logWasUpdated = true;

        // only one console instance adds to the logs, all instances update in next update call
        if (this == main)
        {
            logs.Add(new Log
            {
                message = message,
                stackTrace = stackTrace,
                type = type
            });
        }
    }

    private struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }
}