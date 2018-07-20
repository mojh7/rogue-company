using System;
using System.Diagnostics;

public class DebugX
{
    [Conditional("UnityEditor")]
    public static void Log(object msg)
    {
        UnityEngine.Debug.Log(msg);
    }
    [Conditional("UnityEditor")]
    public static void Log(int msg)
    {
        UnityEngine.Debug.Log(msg);
    }
}
