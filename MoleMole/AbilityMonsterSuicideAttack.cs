namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterSuicideAttack : BaseAbilityMixin
    {
        private float _basicBrightness;
        private string _brightnessPropertyName;
        private bool _isTouchExplode;
        private string _onTouchTriggerID;
        private SkinnedMeshRenderer _skinedMeshRenderer;
        private EntityTimer _suicideTimer;
        private EntityTimer _warningTimer;
        private const float BASIC_WARNING_SPEED_RATIO = 0.15f;
        private MonsterSuicideAttackMixin config;
        private const float MAX_BRIGHTNESS = 6f;
        private const float MIN_WARNING_INTERVAL = 0.1f;
        private const float WARNING_SPEED_RATIO = 0.9f;

        public AbilityMonsterSuicideAttack(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._basicBrightness = 1f;
            this._brightnessPropertyName = "_Emission";
            this.config = (MonsterSuicideAttackMixin) config;
            this._suicideTimer = new EntityTimer(instancedAbility.Evaluate(this.config.SuicideCountDownDuration));
            this._warningTimer = new EntityTimer(instancedAbility.Evaluate(this.config.SuicideCountDownDuration) * 0.15f);
            this._isTouchExplode = this.config.IsTouchExplode;
            this._onTouchTriggerID = this.config.OnTouchTriggerID;
            this._skinedMeshRenderer = base.actor.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (this._skinedMeshRenderer != null)
            {
                this._basicBrightness = this._skinedMeshRenderer.sharedMaterial.GetFloat(this._brightnessPropertyName);
            }
        }

        public override void Core()
        {
            this._suicideTimer.Core(1f);
            this._warningTimer.Core(1f);
            if (this._skinedMeshRenderer != null)
            {
                float t = this._warningTimer.timer / this._warningTimer.timespan;
                float num2 = Mathf.Lerp(this._basicBrightness, 6f, t);
                this._skinedMeshRenderer.sharedMaterial.SetFloat(this._brightnessPropertyName, num2);
            }
            if (this._warningTimer.isTimeUp)
            {
                this._warningTimer.timespan *= 0.9f;
                this._warningTimer.timespan = Mathf.Max(0.1f, this._warningTimer.timespan);
                this._warningTimer.Reset(true);
            }
            if (this._suicideTimer.isTimeUp)
            {
                MonsterActor actor = base.actor as MonsterActor;
                actor.needDropReward = false;
                actor.ForceKill(base.actor.runtimeID, KillEffect.KillImmediately);
                this._suicideTimer.Reset(false);
                this._warningTimer.Reset(false);
            }
            if (!string.IsNullOrEmpty(this._onTouchTriggerID))
            {
                base.actor.entity.ResetTrigger(this._onTouchTriggerID);
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (evt.abilityArgument != null)
            {
                MonsterSuicideAttackMixinArgument abilityArgument = evt.abilityArgument as MonsterSuicideAttackMixinArgument;
                if (abilityArgument != null)
                {
                    this._suicideTimer.timespan = abilityArgument.SuicideCountDown;
                    this._warningTimer.timespan = abilityArgument.BeapInterval;
                    this._isTouchExplode = abilityArgument.SuicideOnTouch;
                    if (!string.IsNullOrEmpty(abilityArgument.OnTouchTriggerID))
                    {
                        this._onTouchTriggerID = abilityArgument.OnTouchTriggerID;
                    }
                }
            }
            this._suicideTimer.Reset(true);
            this._warningTimer.Reset(true);
        }

        public override bool OnEvent(BaseEvent evt)
        {
            if (evt is EvtKilled)
            {
                return this.OnKilled((EvtKilled) evt);
            }
            return ((evt is EvtTouch) && this.OnTouch((EvtTouch) evt));
        }

        private bool OnKilled(EvtKilled evt)
        {
            this._suicideTimer.Reset(false);
            if (evt.killerID == base.actor.runtimeID)
            {
                base.entity.TriggerAttackPattern(this.config.SuicideHitAnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base.actor.runtimeID, MixinTargetting.Enemy));
                if (this.config.SuicideHitAlliedAnimEventID != null)
                {
                    base.entity.TriggerAttackPattern(this.config.SuicideHitAlliedAnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base.actor.runtimeID, MixinTargetting.Allied));
                }
                base.FireMixinEffect(this.config.SuicideEffect, base.entity, false);
            }
            else
            {
                base.entity.TriggerAttackPattern(this.config.KilledHitAnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base.actor.runtimeID, MixinTargetting.Enemy));
                if (this.config.KilledHitAlliedAnimEventID != null)
                {
                    base.entity.TriggerAttackPattern(this.config.KilledHitAlliedAnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base.actor.runtimeID, MixinTargetting.Allied));
                }
                base.FireMixinEffect(this.config.KilledEffect, base.entity, false);
            }
            return true;
        }

        private bool OnTouch(EvtTouch evt)
        {
            if (this._suicideTimer.isActive && this._isTouchExplode)
            {
                base.actor.ForceKill(base.actor.runtimeID, KillEffect.KillImmediately);
                this._suicideTimer.Reset(false);
                this._warningTimer.Reset(false);
            }
            if (this._suicideTimer.isActive && !string.IsNullOrEmpty(this._onTouchTriggerID))
            {
                base.actor.entity.SetTrigger(this._onTouchTriggerID);
            }
            return true;
        }
    }
}

