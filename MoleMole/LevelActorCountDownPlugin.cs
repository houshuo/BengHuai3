namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LevelActorCountDownPlugin : BaseActorPlugin
    {
        private LevelActor _levelActor;
        private float countDownSpeedRatio;
        [HideInInspector]
        public SafeFloat countDownTimer = 0f;
        public bool isTiming;
        [HideInInspector]
        public Action<float, float> OnTimingChange;
        [HideInInspector]
        public Action<bool> OnTimingVisibleChange;
        private float speedRatioInNormalTime;
        private float speedRatioInWitchTime;
        public bool timeUpWin;
        public SafeFloat totalTime = 0f;

        public LevelActorCountDownPlugin(LevelActor levelActor, float totalTime, bool timesUpWin = false)
        {
            this._levelActor = levelActor;
            this.timeUpWin = timesUpWin;
            this.totalTime = totalTime;
            this.isTiming = false;
            this.IsLevelTimeUp = false;
            this.countDownSpeedRatio = 1f;
            this.speedRatioInNormalTime = 1f;
            this.speedRatioInWitchTime = 1f;
            this.OnTimingChange = (Action<float, float>) Delegate.Combine(this.OnTimingChange, new Action<float, float>(this.SetTimingText));
            this.SetTimingText(0f, totalTime);
        }

        public void AddRemainTime(float timeDelta)
        {
            float newValue = this.countDownTimer + timeDelta;
            if (newValue <= 0f)
            {
                newValue = 0f;
                this.isTiming = false;
                this.IsLevelTimeUp = true;
                this.OnTimingChange = (Action<float, float>) Delegate.Remove(this.OnTimingChange, new Action<float, float>(this.SetTimingText));
                Singleton<EventManager>.Instance.FireEvent(new EvtLevelTimesUp(0x21800001), MPEventDispatchMode.Normal);
            }
            DelegateUtils.UpdateField(ref this.countDownTimer, newValue, this.OnTimingChange);
        }

        public override void Core()
        {
            if (this.isTiming && (this.countDownTimer > 0f))
            {
                float newValue = this.countDownTimer - (((Time.deltaTime * this._levelActor.levelEntity.TimeScale) * this._levelActor.levelEntity.AuxTimeScale) * this.countDownSpeedRatio);
                if (newValue <= 0f)
                {
                    newValue = 0f;
                    this.isTiming = false;
                    this.IsLevelTimeUp = true;
                    this.OnTimingChange = (Action<float, float>) Delegate.Remove(this.OnTimingChange, new Action<float, float>(this.SetTimingText));
                    Singleton<EventManager>.Instance.FireEvent(new EvtLevelTimesUp(0x21800001), MPEventDispatchMode.Normal);
                }
                DelegateUtils.UpdateField(ref this.countDownTimer, newValue, this.OnTimingChange);
            }
            base.Core();
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtLevelBuffState)
            {
                if (((evt as EvtLevelBuffState).state == LevelBuffState.Start) && ((evt as EvtLevelBuffState).levelBuff == LevelBuffType.WitchTime))
                {
                    this.countDownSpeedRatio = this.speedRatioInWitchTime;
                }
                if (((evt as EvtLevelBuffState).state == LevelBuffState.Stop) && ((evt as EvtLevelBuffState).levelBuff == LevelBuffType.WitchTime))
                {
                    this.countDownSpeedRatio = this.speedRatioInNormalTime;
                }
            }
            return false;
        }

        public override void OnAdded()
        {
            this.countDownTimer = this.totalTime;
        }

        public override void OnRemoved()
        {
        }

        public void ResetPlugin(float totalTime)
        {
            this.totalTime = totalTime;
            this.isTiming = false;
            this.IsLevelTimeUp = false;
            this.countDownTimer = this.totalTime;
            this.SetTimingText(totalTime, totalTime);
        }

        public void SetCountDownSpeedRatio(float ratioInNormalTime, float ratioInWitchTime)
        {
            this.speedRatioInNormalTime = ratioInNormalTime;
            this.speedRatioInWitchTime = ratioInWitchTime;
            if (Singleton<LevelManager>.Instance.levelActor.IsLevelBuffActive(LevelBuffType.WitchTime))
            {
                this.countDownSpeedRatio = ratioInWitchTime;
            }
            else
            {
                this.countDownSpeedRatio = this.speedRatioInNormalTime;
            }
        }

        private void SetTimingText(float oldTimer, float newTimer)
        {
            if (Mathf.CeilToInt(oldTimer) != Mathf.CeilToInt(newTimer))
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetTimeCountDownText, newTimer));
            }
        }

        public bool IsLevelTimeUp { get; private set; }
    }
}

