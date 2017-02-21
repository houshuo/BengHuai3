namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoNumberJump : MonoBehaviour
    {
        private bool _isPlaying;
        private bool _isTimeFormat;
        private bool _playAlready;
        private int _targetValue;
        private float _updateInterval;
        private float _updateTimer;
        private int currentValue;
        public bool replay;
        public float showTime = 2f;
        public Text text;
        public int valueChangeTimesPerSecond = 20;
        public int valueDelta = 1;

        private void SetInitValue()
        {
            int num = this.valueChangeTimesPerSecond * this.valueDelta;
            int num2 = this._targetValue / num;
            if (num2 > this.showTime)
            {
                this.currentValue = Mathf.FloorToInt(this._targetValue - (num * this.showTime));
            }
            else
            {
                this.currentValue = 0;
            }
            this._updateInterval = 1f / ((float) this.valueChangeTimesPerSecond);
            if (this._targetValue < (this.valueDelta * this.valueChangeTimesPerSecond))
            {
                this.valueDelta = Mathf.Clamp(this._targetValue / this.valueChangeTimesPerSecond, 1, this.valueDelta + 1);
            }
            this._updateTimer = 0f;
            this.ShowCurrentValue();
        }

        public void SetTargetValue(int targetValue, bool isTimeFormat = false, bool startPlay = false)
        {
            this._targetValue = targetValue;
            this._isTimeFormat = isTimeFormat;
            if (startPlay)
            {
                this.replay = true;
            }
        }

        private void ShowCurrentValue()
        {
            if (this._isTimeFormat)
            {
                int num = Mathf.CeilToInt((float) this.currentValue) / 60;
                int num2 = Mathf.CeilToInt((float) this.currentValue) - (60 * num);
                this.text.text = string.Format("{0:D2}:{1:D2}", num, num2);
            }
            else
            {
                this.text.text = this.currentValue.ToString();
            }
        }

        private void Update()
        {
            if (this.replay && !this._playAlready)
            {
                this.SetInitValue();
                this.replay = false;
                this._isPlaying = true;
                this._playAlready = true;
            }
            if (this._isPlaying)
            {
                this._updateTimer += Time.deltaTime;
                if (this._updateTimer > this._updateInterval)
                {
                    this.currentValue += this.valueDelta;
                    if (this.currentValue >= this._targetValue)
                    {
                        this.currentValue = this._targetValue;
                        this._isPlaying = false;
                    }
                    this.ShowCurrentValue();
                    this._updateTimer = 0f;
                }
            }
        }
    }
}

