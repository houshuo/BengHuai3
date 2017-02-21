namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ConfigCloudStyle
    {
        public Color BrightColor;
        public Color DarkColor;
        public Vector3 FlashAttenuationFactors;
        public Color FlashColor;
        public Color RimColor;
        public Color SecondDarkColor;

        public void CopyFrom(ConfigCloudStyle other)
        {
            if (other != null)
            {
                this.BrightColor = other.BrightColor;
                this.DarkColor = other.DarkColor;
                this.SecondDarkColor = other.SecondDarkColor;
                this.RimColor = other.RimColor;
                this.FlashColor = other.FlashColor;
                this.FlashAttenuationFactors = other.FlashAttenuationFactors;
            }
        }

        public static ConfigCloudStyle Lerp(ConfigCloudStyle config1, ConfigCloudStyle config2, float t)
        {
            return new ConfigCloudStyle { BrightColor = Color.Lerp(config1.BrightColor, config2.BrightColor, t), DarkColor = Color.Lerp(config1.DarkColor, config2.DarkColor, t), SecondDarkColor = Color.Lerp(config1.SecondDarkColor, config2.SecondDarkColor, t), RimColor = Color.Lerp(config1.RimColor, config2.RimColor, t), FlashColor = Color.Lerp(config1.FlashColor, config2.FlashColor, t), FlashAttenuationFactors = Vector3.Lerp(config1.FlashAttenuationFactors, config2.FlashAttenuationFactors, t) };
        }
    }
}

