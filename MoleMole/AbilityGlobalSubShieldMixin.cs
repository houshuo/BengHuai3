namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityGlobalSubShieldMixin : BaseAbilityMixin
    {
        private MixinEffect _currentMixinEffect;
        private DynamicActorValue<float> _globalShieldValue;
        private int _shieldEffectPatternIx;
        private float[] _shieldEffectRange;
        private float _totalShieldValue;
        private GlobalSubShieldMixin config;
        public static string GLOBAL_SHIELD_KEY = "GlobalShield";

        public AbilityGlobalSubShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._shieldEffectPatternIx = -1;
            this.config = (GlobalSubShieldMixin) config;
        }

        public override void OnAdded()
        {
            this._globalShieldValue = base.instancedAbility.caster.abilityPlugin.CreateOrGetDynamicFloat(GLOBAL_SHIELD_KEY, 0f);
            this._globalShieldValue.SubAttach(new Action<float, float>(this.OnShieldChanged), ref this._totalShieldValue);
            if (this.config.ShieldEffects != null)
            {
                this._shieldEffectRange = new float[this.config.ShieldEffectRanges.Length + 1];
                this.config.ShieldEffectRanges.CopyTo(this._shieldEffectRange, 0);
                this._shieldEffectRange[this.config.ShieldEffectRanges.Length] = 1f;
            }
            this.UpdateShieldEffect();
        }

        private bool OnPostBeingHit(EvtBeingHit evt)
        {
            if (!evt.attackData.isAnimEventAttack)
            {
                return false;
            }
            if (evt.attackData.rejected)
            {
                return false;
            }
            if (base.actor.abilityState.ContainsState(AbilityState.Invincible) || base.actor.abilityState.ContainsState(AbilityState.Undamagable))
            {
                return false;
            }
            if ((this._globalShieldValue == null) || (this._globalShieldValue.Value <= 0f))
            {
                return false;
            }
            float num = this._globalShieldValue.Value;
            float num2 = evt.attackData.damage * (1f - DamageModelLogic.GetDefenceRatio(base.instancedAbility.caster.defense * base.instancedAbility.Evaluate(this.config.ShieldDefenceRatio), evt.attackData.attackerLevel));
            float newValue = num - num2;
            bool flag = Mathf.Approximately(evt.attackData.GetTotalDamage() - evt.attackData.damage, 0f);
            if (newValue <= 0f)
            {
                newValue = 0f;
            }
            this._globalShieldValue.Pub(newValue);
            if (newValue > 0f)
            {
                if (flag)
                {
                    evt.attackData.Reject(AttackResult.RejectType.RejectAll);
                }
                else
                {
                    evt.attackData.damage = 0f;
                    evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Mute;
                }
                if (evt.attackData.hitCollision != null)
                {
                    base.FireMixinEffect(this.config.ShieldSuccessEffect, base.entity, evt.attackData.hitCollision.hitPoint, evt.attackData.hitCollision.hitDir, false);
                }
                else
                {
                    base.FireMixinEffect(this.config.ShieldSuccessEffect, base.entity, false);
                }
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ShieldSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
            }
            else if (num > 0f)
            {
                float num4 = 1f - (num / evt.attackData.damage);
                evt.attackData.damage *= num4;
                if (this.config.ShieldBrokenTimeSlow > 0f)
                {
                    Singleton<LevelManager>.Instance.levelActor.TimeSlow(this.config.ShieldBrokenTimeSlow);
                }
                base.FireMixinEffect(this.config.ShieldBrokenEffect, base.entity, false);
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ShieldBrokenActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
                evt.attackData.frameHalt = 0;
                evt.attackData.attackerAniDamageRatio = 10f;
                Singleton<EventManager>.Instance.FireEvent(new EvtShieldBroken(base.actor.runtimeID), MPEventDispatchMode.Normal);
                evt.attackData.AddHitFlag(AttackResult.ActorHitFlag.Count);
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnPostBeingHit((EvtBeingHit) evt));
        }

        public override void OnRemoved()
        {
            this._globalShieldValue.SubDetach(new Action<float, float>(this.OnShieldChanged));
            this.RemoveEffect();
        }

        private void OnShieldChanged(float from, float to)
        {
            this.UpdateShieldEffect();
        }

        private void RemoveEffect()
        {
            if (this._currentMixinEffect != null)
            {
                base.entity.DetachEffect(this._shieldEffectPatternIx);
            }
            this._currentMixinEffect = null;
        }

        private void UpdateShieldEffect()
        {
            MixinEffect effect = this.config.ShieldEffects[0];
            float a = this._globalShieldValue.Value / this._totalShieldValue;
            if (Mathf.Approximately(a, 0f))
            {
                this.RemoveEffect();
            }
            else
            {
                for (int i = 0; i < this._shieldEffectRange.Length; i++)
                {
                    if (a <= this._shieldEffectRange[i])
                    {
                        effect = this.config.ShieldEffects[i];
                        break;
                    }
                }
                if (this._currentMixinEffect != effect)
                {
                    if (this._currentMixinEffect != null)
                    {
                        base.entity.DetachEffect(this._shieldEffectPatternIx);
                    }
                    this._currentMixinEffect = effect;
                    this._shieldEffectPatternIx = base.entity.AttachEffect(effect.EffectPattern);
                }
            }
        }
    }
}

