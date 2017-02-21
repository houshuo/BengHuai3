namespace MoleMole
{
    using System;
    using UnityEngine;

    public sealed class MonoFPSIndicator : MonoBehaviour
    {
        private int _frames;
        private float _time;
        private float _timeleft;
        private int _totalFrames;
        private float _totalTime;
        public float fpsAvg;
        public float fpsMax = float.MinValue;
        public float fpsMin = float.MaxValue;
        public string logContext = string.Empty;
        public float updateInterval = 0.5f;

        public void Start()
        {
            this._time = 0f;
            this._frames = 0;
            this._totalTime = 0f;
            this._totalFrames = 0;
            if (base.GetComponent<GUIText>() != null)
            {
                this._timeleft = this.updateInterval;
            }
        }

        public void Update()
        {
            this._timeleft -= Time.deltaTime;
            if (Time.deltaTime > 0f)
            {
                this._time += Time.unscaledDeltaTime;
                this._totalTime += Time.unscaledDeltaTime;
            }
            this._frames++;
            this._totalFrames++;
            if (this._timeleft <= 0f)
            {
                float num = ((float) this._frames) / this._time;
                if (num < this.fpsMin)
                {
                    this.fpsMin = num;
                }
                if (num > this.fpsMax)
                {
                    this.fpsMax = num;
                }
                base.transform.Find("Text").GetComponent<Text>().text = string.Empty + num.ToString("f2");
                this._timeleft = this.updateInterval;
                this._time = 0f;
                this._frames = 0;
            }
        }
    }
}

