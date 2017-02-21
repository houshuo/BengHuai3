namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ConfigBackground
    {
        public Color BColor;
        public float BloomFactor;
        public Color GColor;
        public Color GradBottomColor;
        [Range(0.01f, 10f)]
        public float GradHigh;
        [Range(-5f, 1f)]
        public float GradLocation;
        public Color GradTopColor;
        [Range(0.01f, 1f)]
        public float High;
        public Color RColor;
        [Range(0f, 10f)]
        public float SecTexEmission;
        [Range(0.01f, 1f)]
        public float SecTexHigh;
        [Range(0f, 1f)]
        public float SecTexXLocation;
        [Range(0f, 1f)]
        public float SecTexYLocation;
        [Range(0f, 1f)]
        public float XLocation;
        [Range(0f, 1f)]
        public float YLocation;

        public void CopyFrom(ConfigBackground other)
        {
            if (other != null)
            {
                this.RColor = other.RColor;
                this.GColor = other.GColor;
                this.BColor = other.BColor;
                this.GradTopColor = other.GradTopColor;
                this.GradBottomColor = other.GradBottomColor;
                this.XLocation = other.XLocation;
                this.YLocation = other.YLocation;
                this.High = other.High;
                this.SecTexXLocation = other.SecTexXLocation;
                this.SecTexYLocation = other.SecTexYLocation;
                this.SecTexHigh = other.SecTexHigh;
                this.SecTexEmission = other.SecTexEmission;
                this.GradLocation = other.GradLocation;
                this.GradHigh = other.GradHigh;
                this.BloomFactor = other.BloomFactor;
            }
        }

        public static ConfigBackground Lerp(ConfigBackground config1, ConfigBackground config2, float t)
        {
            return new ConfigBackground { RColor = Color.Lerp(config1.RColor, config2.RColor, t), GColor = Color.Lerp(config1.GColor, config2.GColor, t), BColor = Color.Lerp(config1.BColor, config2.BColor, t), GradTopColor = Color.Lerp(config1.GradTopColor, config2.GradTopColor, t), GradBottomColor = Color.Lerp(config1.GradBottomColor, config2.GradBottomColor, t), XLocation = Mathf.Lerp(config1.XLocation, config2.XLocation, t), YLocation = Mathf.Lerp(config1.YLocation, config2.YLocation, t), High = Mathf.Lerp(config1.High, config2.High, t), SecTexXLocation = Mathf.Lerp(config1.SecTexXLocation, config2.SecTexXLocation, t), SecTexYLocation = Mathf.Lerp(config1.SecTexYLocation, config2.SecTexYLocation, t), SecTexHigh = Mathf.Lerp(config1.SecTexHigh, config2.SecTexHigh, t), SecTexEmission = Mathf.Lerp(config1.SecTexEmission, config2.SecTexEmission, t), GradLocation = Mathf.Lerp(config1.GradLocation, config2.GradLocation, t), GradHigh = Mathf.Lerp(config1.GradHigh, config2.GradHigh, t), BloomFactor = Mathf.Lerp(config1.BloomFactor, config2.BloomFactor, t) };
        }
    }
}

