namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityDefendWithShieldMixin : AbilityDefendWithDirectionMixin
    {
        private DefendWithShieldMixin config;
        public float maxShield;
        public float shield;

        public AbilityDefendWithShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (DefendWithShieldMixin) config;
        }

        public override void Core()
        {
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this.shield = this.maxShield;
            (base.actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(this.config.ShieldRatioAnimatorParam, this.shield / this.maxShield, false);
        }

        public override void OnAdded()
        {
            this.shield = this.maxShield = base.actor.baseMaxHP * base.instancedAbility.Evaluate(this.config.ShieldHPRatio);
            (base.actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(this.config.ShieldRatioAnimatorParam, this.shield / this.maxShield, false);
        }

        protected override bool OnBeingHit(EvtBeingHit evt)
        {
            float totalDamage;
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
            if ((actor == null) || (actor.entity == null))
            {
                return false;
            }
            if (this.config.DefendElemental)
            {
                totalDamage = evt.attackData.GetTotalDamage();
            }
            else
            {
                totalDamage = evt.attackData.damage;
            }
            float num2 = totalDamage * Mathf.Pow(evt.attackData.attackerAniDamageRatio, this.config.ShieldAniDamageRatioPow);
            if (base.CheckSkillID(evt))
            {
                if (base.CheckAngle(evt))
                {
                    this.shield -= num2;
                    if (this.shield <= 0f)
                    {
                        base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ShieldBrokenActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
                        this.shield = this.maxShield;
                    }
                    else
                    {
                        base.DefendSuccess(evt);
                    }
                    (base.actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(this.config.ShieldRatioAnimatorParam, this.shield / this.maxShield, false);
                }
                else
                {
                    base.DefendFailure(evt);
                }
            }
            return true;
        }
    }
}

