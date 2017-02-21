namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class MonoHitableProp : BaseMonoPropObject, IFrameHaltable
    {
        protected FrameHaltPlugin _frameHaltPlugin;
        public Collider hitbox;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity, uint sourceID)
        {
            this.ResetTrigger("LightHitTrigger");
            this.ResetTrigger("HitTrigger");
            if (hitEffect > AttackResult.AnimatorHitEffect.Normal)
            {
                this.SetTrigger("HitTrigger");
            }
            else
            {
                this.SetTrigger("LightHitTrigger");
            }
            this.FrameHalt(frameHalt);
        }

        public override void FrameHalt(int frameNum)
        {
            if (frameNum > 0)
            {
                this._frameHaltPlugin.FrameHalt(frameNum);
            }
        }

        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
            base.onIsGhostChanged = (Action<bool>) Delegate.Combine(base.onIsGhostChanged, new Action<bool>(this.OnIsGhostChanged));
            this.InitPlugins();
        }

        protected virtual void InitPlugins()
        {
            this._frameHaltPlugin = new FrameHaltPlugin(this);
        }

        uint IFrameHaltable.GetRuntimeID()
        {
            return base.GetRuntimeID();
        }

        private void OnIsGhostChanged(bool isGhost)
        {
            this.hitbox.enabled = !isGhost;
        }

        protected override void OnTimeScaleChanged(float newTimeScale)
        {
            if (base.animator != null)
            {
                base.animator.speed = newTimeScale;
            }
        }

        public override void SetDied(KillEffect killEffect)
        {
            if ((base.config.PropArguments.OnKillEffectPattern != null) && base.gameObject.activeSelf)
            {
                this.FireEffect(base.config.PropArguments.OnKillEffectPattern);
            }
            base.SetDied(killEffect);
        }

        protected override void Update()
        {
            this.UpdatePlugins();
            base.Update();
        }

        protected void UpdatePlugins()
        {
            this._frameHaltPlugin.Core();
        }

        public FixedStack<float> timeScaleStack
        {
            get
            {
                return base._timeScaleStack;
            }
        }
    }
}

