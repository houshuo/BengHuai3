namespace MoleMole
{
    using System;
    using UnityEngine;

    public static class MobileDeviceSetupUtil
    {
        [RuntimeInitializeOnLoadMethod]
        public static void IOSDeviceInitialize()
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }
    }
}

