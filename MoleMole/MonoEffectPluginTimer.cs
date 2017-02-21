namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginTimer : BaseMonoEffectPlugin
    {
        private bool _isToBeRemove;
        private bool _needStop;
        private float _timer;
        [Header("How long does this effect last.")]
        public float EffectTime;
        [Header("Kill Immediately instead of stop emitting")]
        public bool KillImmediately;

        protected override void Awake()
        {
            base.Awake();
            this._timer = 0f;
            this._needStop = true;
            this._isToBeRemove = false;
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemove;
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
            this._timer = 0f;
            this._needStop = true;
            this._isToBeRemove = false;
        }

        public void Update()
        {
            if (this._timer < this.EffectTime)
            {
                this._timer += Time.deltaTime * base._effect.TimeScale;
            }
            if (this._timer >= this.EffectTime)
            {
                if (this._needStop)
                {
                    if (this.KillImmediately || (base._effect.mainParticleSystem == null))
                    {
                        base._effect.SetDestroyImmediately();
                    }
                    else
                    {
                        base._effect.SetDestroy();
                    }
                }
                this._needStop = false;
            }
        }
    }
}

