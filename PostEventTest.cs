using System;
using UnityEngine;

public class PostEventTest : MonoBehaviour
{
    public void PostEvt()
    {
        AkSoundEngine.PostEvent("TestPlay", base.gameObject);
    }
}

