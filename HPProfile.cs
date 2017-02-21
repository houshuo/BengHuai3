using System;
using System.Diagnostics;
using UnityEngine;

public static class HPProfile
{
    private static Stopwatch stopWatch = new Stopwatch();

    public static void Begin()
    {
        stopWatch.Reset();
        stopWatch.Start();
    }

    public static void End(string prompt)
    {
        stopWatch.Stop();
        float num = (stopWatch.ElapsedTicks * 1000f) / ((float) Stopwatch.Frequency);
        UnityEngine.Debug.Log(string.Format("{0} : {1}", prompt, num.ToString()));
    }
}

