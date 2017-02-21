namespace MoleMole
{
    using System;

    [Serializable]
    public class WeatherInfo
    {
        public string extraInfo;
        public DateTime infoTime;
        public float temperature;
        public WeatherType weatherType;

        public override string ToString()
        {
            object[] args = new object[] { this.weatherType, this.temperature.ToString(), this.infoTime.ToString(), (this.extraInfo != null) ? this.extraInfo : "<NULL>" };
            return string.Format("[{0}] {1} ({2}) : {3}", args);
        }
    }
}

