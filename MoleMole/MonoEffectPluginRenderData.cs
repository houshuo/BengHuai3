namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class MonoEffectPluginRenderData : BaseMonoEffectPlugin
    {
        private EntityTimer[] _timers;
        public TintCameraWithDelay[] tintEntries;

        protected override void Awake()
        {
            base.Awake();
            this._timers = new EntityTimer[this.tintEntries.Length];
            for (int i = 0; i < this._timers.Length; i++)
            {
                this._timers[i] = new EntityTimer(this.tintEntries[i].Delay, base._effect);
                this._timers[i].Reset(false);
            }
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        public override void SetDestroy()
        {
            for (int i = 0; i < this._timers.Length; i++)
            {
                this._timers[i].Reset(false);
            }
        }

        public override void Setup()
        {
            for (int i = 0; i < this._timers.Length; i++)
            {
                this._timers[i].Reset(true);
            }
        }

        private void Update()
        {
            for (int i = 0; i < this._timers.Length; i++)
            {
                this._timers[i].Core(1f);
                if (this._timers[i].isTimeUp)
                {
                    TintCameraWithDelay delay = this.tintEntries[i];
                    Singleton<StageManager>.Instance.GetPerpStage().TriggerTint(delay.RenderDataName, delay.Duration, delay.TransitDuration);
                    this._timers[i].Reset(false);
                }
            }
        }

        [Serializable]
        public class TintCameraWithDelay : ConfigTintCamera
        {
            public float Delay;
        }
    }
}

