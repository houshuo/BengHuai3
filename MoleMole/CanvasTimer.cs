namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CanvasTimer
    {
        private float _timer = 0f;
        private float _triggerTimer = 0f;
        public bool infiniteTimeSpan;
        private bool isRunning;
        public float timespan;
        public Action timeTriggerCallback;
        public Action timeUpCallback;
        public float triggerCD;

        public CanvasTimer()
        {
            this.IsTimeUp = false;
            this.isRunning = true;
        }

        public void Core()
        {
            if (!this.IsTimeUp && this.isRunning)
            {
                if (!this.infiniteTimeSpan)
                {
                    this._timer += Time.deltaTime;
                    if (this._timer > this.timespan)
                    {
                        this._timer = this.timespan;
                        this.IsTimeUp = true;
                        if (this.timeUpCallback != null)
                        {
                            this.timeUpCallback();
                        }
                    }
                }
                if (this.triggerCD > 0f)
                {
                    if (this._triggerTimer > this.triggerCD)
                    {
                        this._triggerTimer -= this.triggerCD;
                        if (this.timeTriggerCallback != null)
                        {
                            this.timeTriggerCallback();
                        }
                    }
                    this._triggerTimer += Time.deltaTime;
                }
            }
        }

        public void Destroy()
        {
            this.IsTimeUp = true;
            this.timeUpCallback = null;
            this.timeTriggerCallback = null;
        }

        public void StartRun(bool reset = false)
        {
            this.isRunning = true;
            if (reset)
            {
                this._triggerTimer = 0f;
                this._timer = 0f;
                this.IsTimeUp = false;
            }
        }

        public void StopRun()
        {
            this.isRunning = false;
        }

        public bool IsTimeUp { get; private set; }
    }
}

