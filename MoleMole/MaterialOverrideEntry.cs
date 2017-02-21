namespace MoleMole
{
    using System;
    using UnityEngine;

    [Serializable]
    public class MaterialOverrideEntry
    {
        public static MaterialOverrideEntry[] EMPTY = new MaterialOverrideEntry[0];
        public Material material;
        public string materialOverrideKey;
    }
}

