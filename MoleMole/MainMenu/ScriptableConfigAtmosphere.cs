namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    public class ScriptableConfigAtmosphere : ScriptableObject
    {
        public ConfigBackground Background = new ConfigBackground();
        public ConfigCloudStyle CloudStyle;
        [HideInInspector]
        public float FrameTime;
        public ConfigIndoor Indoor;
    }
}

