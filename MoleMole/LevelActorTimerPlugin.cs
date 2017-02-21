namespace MoleMole
{
    using System;
    using UnityEngine;

    public class LevelActorTimerPlugin : BaseActorPlugin
    {
        private LevelActor _levelActor;
        private SafeFloat _timer = 0f;
        private bool _timing;

        public LevelActorTimerPlugin(LevelActor levelActor)
        {
            this._levelActor = levelActor;
            this._timing = false;
        }

        public override void Core()
        {
            if (this._timing)
            {
                float oldTimer = (float) this._timer;
                this._timer += Time.deltaTime * this._levelActor.levelEntity.TimeScale;
                this.SetTimingText(oldTimer, (float) this._timer);
            }
            base.Core();
        }

        public override void OnAdded()
        {
            this._timer = 0f;
        }

        private void SetTimingText(float oldTimer, float newTimer)
        {
            if (Mathf.CeilToInt(oldTimer) != Mathf.CeilToInt(newTimer))
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetTimerText, newTimer));
            }
        }

        public void StartTiming()
        {
            this._timing = true;
        }

        public void StopTiming()
        {
            this._timing = false;
        }

        public float Timer
        {
            get
            {
                return (float) this._timer;
            }
        }
    }
}

