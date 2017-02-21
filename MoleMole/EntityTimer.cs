namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class EntityTimer
    {
        private BaseMonoEntity _timeScaleEntity;
        public bool isTimeUp;
        public float timer;
        public float timespan;
        public Action timeupAction;

        public EntityTimer() : this(0f, Singleton<LevelManager>.Instance.levelEntity, false)
        {
            this.SetActive(false);
        }

        public EntityTimer(float timespan) : this(timespan, Singleton<LevelManager>.Instance.levelEntity, false)
        {
        }

        public EntityTimer(float timespan, BaseMonoEntity timeScaleEntity) : this(timespan, timeScaleEntity, false)
        {
        }

        public EntityTimer(float timespan, BaseMonoEntity timeScaleEntity, bool active)
        {
            this._timeScaleEntity = timeScaleEntity;
            this.timespan = timespan;
            this.Reset(active);
        }

        public void Core(float deltaTimeRatio = 1)
        {
            if (this.isActive && !this.isTimeUp)
            {
                float num = (Time.deltaTime * this._timeScaleEntity.TimeScale) * deltaTimeRatio;
                this.timer += num;
                if (this.timer > this.timespan)
                {
                    this.timer = this.timespan;
                    this.isTimeUp = true;
                    if (this.timeupAction != null)
                    {
                        this.timeupAction();
                    }
                }
            }
        }

        public float GetTimingRatio()
        {
            return Mathf.Clamp01(this.timer / this.timespan);
        }

        public void Reset()
        {
            this.timer = 0f;
            this.isTimeUp = false;
        }

        public void Reset(bool active)
        {
            this.Reset();
            this.SetActive(active);
        }

        public void SetActive(bool active)
        {
            this.isActive = active;
        }

        public bool isActive { get; private set; }
    }
}

