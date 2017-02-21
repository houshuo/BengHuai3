namespace MoleMole
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class MonoSpikeTriggerProp : MonoTriggerUnitFieldProp
    {
        private Animation[] _animations;
        private float _CD = 2f;
        private string _disableAnimationName = "Disable";
        private float _duration = 2f;
        private string _enableAnimationName = "Enable";
        private bool _isAttacking;
        private bool _isInContineousState;
        private float effectDelayTimer;
        private const float LoseEffecctDelay = 0.2f;
        private const float TakeEffecctDelay = 0.7f;
        private float timer;

        private void DisableCollider()
        {
            base.ClearInsideColliders();
            base._triggerCollider.enabled = false;
        }

        private void DisableSprike()
        {
            if (base._triggerCollider.enabled)
            {
                foreach (Animation animation in this._animations)
                {
                    animation.Play(this._disableAnimationName);
                }
            }
        }

        private void EnableCollider()
        {
            base.ClearInsideColliders();
            base._triggerCollider.enabled = true;
        }

        private void EnableSprike()
        {
            if (!base._triggerCollider.enabled)
            {
                foreach (Animation animation in this._animations)
                {
                    animation.Play(this._enableAnimationName);
                }
            }
        }

        public override void InitUnitFieldPropRange(int numberX, int numberZ)
        {
            base.InitUnitFieldPropRange(numberX, numberZ);
            this._animations = base.GetComponentsInChildren<Animation>();
            this._CD = (base.config.PropArguments.CD <= 0f) ? this._CD : base.config.PropArguments.CD;
            this.timer = this._CD;
            this._duration = (base.config.PropArguments.EffectDuration <= 0f) ? this._duration : base.config.PropArguments.EffectDuration;
        }

        protected override void OnTimeScaleChanged(float newTimeScale)
        {
            foreach (Animation animation in this._animations)
            {
                IEnumerator enumerator = animation.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        AnimationState current = (AnimationState) enumerator.Current;
                        current.speed = newTimeScale;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        }

        public void SetContinuousState(bool Active)
        {
            this._isInContineousState = true;
            if (Active)
            {
                foreach (Animation animation in this._animations)
                {
                    animation.Play(this._enableAnimationName);
                }
                base.ClearInsideColliders();
                base._triggerCollider.enabled = true;
            }
            else
            {
                foreach (Animation animation2 in this._animations)
                {
                    animation2.Play(this._disableAnimationName);
                }
                base._triggerCollider.enabled = false;
            }
        }

        public void SetSpikePropDurationAndCD(float duration, float cd)
        {
            this._isInContineousState = false;
            this._duration = duration;
            this._CD = cd;
        }

        protected override void Update()
        {
            base.Update();
            if (!this._isInContineousState)
            {
                this.timer -= Time.deltaTime * this.TimeScale;
                if (this.timer < 0f)
                {
                    if (this._isAttacking)
                    {
                        this.timer = this._duration;
                        this.DisableSprike();
                        this.effectDelayTimer = 0.2f;
                    }
                    else
                    {
                        this.timer = this._CD;
                        this.EnableSprike();
                        this.effectDelayTimer = 0.7f;
                    }
                    this._isAttacking = !this._isAttacking;
                }
                if (this.effectDelayTimer > 0f)
                {
                    this.effectDelayTimer -= Time.deltaTime * this.TimeScale;
                    if (this.effectDelayTimer < 0f)
                    {
                        if (this._isAttacking)
                        {
                            this.EnableCollider();
                        }
                        else
                        {
                            this.DisableCollider();
                        }
                    }
                }
            }
        }
    }
}

