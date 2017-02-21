using System;
using UnityEngine;

public class ClipboardManager
{
    public static void CopyToClipboard(string input)
    {
        AndroidJavaObject obj2 = new AndroidJavaObject("com.mihoyo.ClipboardTools", new object[0]);
        AndroidJavaObject currentActivity = GetCurrentActivity();
        if (currentActivity != null)
        {
            object[] args = new object[] { currentActivity, input };
            obj2.Call("copyTextToClipboard", args);
        }
    }

    public static AndroidJavaObject GetCurrentActivity()
    {
        return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
    }

    public static string GetTextFromClipboard()
    {
        AndroidJavaObject obj2 = new AndroidJavaObject("com.mihoyo.ClipboardTools", new object[0]);
        if (GetCurrentActivity() == null)
        {
            return null;
        }
        return obj2.Call<string>("getTextFromClipboard", new object[0]);
    }
}

