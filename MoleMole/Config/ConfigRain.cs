namespace MoleMole.Config
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ConfigRain : ScriptableObject
    {
        public string audioName;
        public float audioPitch;
        public float audioVolumn;
        public float density;
        public float opaqueness;
        public float size;
        public float speed;
        public float speedStrech;
        public float splashDensity;
        public float splashOpaqueness;

        public ConfigRain Clone()
        {
            return UnityEngine.Object.Instantiate<ConfigRain>(this);
        }

        public void CopyFrom(ConfigRain source)
        {
            this.density = source.density;
            this.splashDensity = source.splashDensity;
            this.speed = source.speed;
            this.speedStrech = source.speedStrech;
            this.size = source.size;
            this.opaqueness = source.opaqueness;
            this.splashOpaqueness = source.splashOpaqueness;
            this.audioName = source.audioName;
            this.audioVolumn = source.audioVolumn;
            this.audioPitch = source.audioPitch;
        }

        public static ConfigRain CreatDefault()
        {
            return new ConfigRain { density = 0f, splashDensity = 0.796f, speed = 1f, speedStrech = 0.02f, size = 0.007f, opaqueness = 1f, splashOpaqueness = 1f, audioVolumn = 1f, audioPitch = 1f };
        }

        public ConfigRain GetNullLerpAble()
        {
            ConfigRain rain = this.Clone();
            rain.density = 0f;
            rain.splashDensity = 0f;
            rain.audioName = this.audioName;
            rain.audioVolumn = this.audioVolumn * 0.7f;
            rain.audioPitch = this.audioPitch * 0.3f;
            return rain;
        }

        public static ConfigRain Lerp(ConfigRain config1, ConfigRain config2, float t)
        {
            if ((config1 == null) && (config2 == null))
            {
                return null;
            }
            ConfigRain rain = ScriptableObject.CreateInstance<ConfigRain>();
            rain.density = Mathf.Lerp(config1.density, config2.density, t);
            rain.splashDensity = Mathf.Lerp(config1.splashDensity, config2.splashDensity, t);
            rain.speed = Mathf.Lerp(config1.speed, config2.speed, t);
            rain.speedStrech = Mathf.Lerp(config1.speedStrech, config2.speedStrech, t);
            rain.size = Mathf.Lerp(config1.size, config2.size, t);
            rain.opaqueness = Mathf.Lerp(config1.opaqueness, config2.opaqueness, t);
            rain.splashOpaqueness = Mathf.Lerp(config1.splashOpaqueness, config2.splashOpaqueness, t);
            rain.audioVolumn = Mathf.Lerp(config1.audioVolumn, config2.audioVolumn, t);
            rain.audioPitch = Mathf.Lerp(config1.audioPitch, config2.audioPitch, t);
            rain.audioName = config1.audioName;
            return rain;
        }
    }
}

