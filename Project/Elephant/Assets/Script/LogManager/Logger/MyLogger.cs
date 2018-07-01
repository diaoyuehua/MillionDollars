using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLogger : ILogHandler
{
    private FileStream fileStream = null;
    private StreamWriter streamWriter = null;
    private static ILogHandler unityLogHandler = null;
    private static MyLogger instance = null;

    public static void Init()
    {
        if (instance == null)
        {
            instance = new MyLogger();
        }

        unityLogHandler = UnityEngine.Debug.unityLogger.logHandler;
        UnityEngine.Debug.unityLogger.logHandler = instance;
    }

    public MyLogger()
    {
        string filePath = Application.persistentDataPath + "/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";

        fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        streamWriter = new StreamWriter(fileStream);

        Application.logMessageReceived += delegate (string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + type.ToString() + " " + condition);
                streamWriter.Write(stackTrace);
                streamWriter.Flush();
            }
        };
    }

    ~MyLogger()
    {
        if(fileStream != null)
        {
            fileStream.Close();
        }
        if (streamWriter != null)
        {
            streamWriter.Close();
        }
    }

    public static void Debug(string format, params object[] args)
    {
        if (Application.isEditor)
        {
            UnityEngine.Debug.unityLogger.Log(string.Format(format, args));
        }
        else
        {
            unityLogHandler.LogFormat(LogType.Log, null, format, args);
        }
    }

    public static void Info(string tag, string format, params object[] args)
    {
        UnityEngine.Debug.unityLogger.Log(tag, string.Format(format, args));
    }

    public static void Warn(string tag, string format, params object[] args)
    {
        UnityEngine.Debug.unityLogger.LogWarning(tag, string.Format(format, args));
    }

    public static void Error(string tag, string format, params object[] args)
    {
        UnityEngine.Debug.unityLogger.LogError(tag, string.Format(format, args));
    }

    void ILogHandler.LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        if (logType != LogType.Exception)
        {
            streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + logType.ToString() + " " + string.Format(format, args));
        }
        unityLogHandler.LogFormat(logType, context, format, args);
    }

    void ILogHandler.LogException(Exception exception, UnityEngine.Object context)
    {
        unityLogHandler.LogException(exception, context);
    }
}
