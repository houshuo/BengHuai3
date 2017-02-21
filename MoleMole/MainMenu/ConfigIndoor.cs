namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ConfigIndoor
    {
        public Color TintColor;

        public void CopyFrom(ConfigIndoor other)
        {
            if (other != null)
            {
                this.TintColor = other.TintColor;
            }
        }

        public static ConfigIndoor Lerp(ConfigIndoor config1, ConfigIndoor config2, float t)
        {
            return new ConfigIndoor { TintColor = Color.Lerp(config1.TintColor, config2.TintColor, t) };
        }
    }
}

