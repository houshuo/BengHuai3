namespace MoleMole
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ColorOverrideEntry
    {
        public Color color;
        public string colorOverrideKey;
        public static ColorOverrideEntry[] EMPTY = new ColorOverrideEntry[0];
    }
}

