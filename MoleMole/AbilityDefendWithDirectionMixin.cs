namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityDefendWithDirectionMixin : BaseAbilityMixin
    {
        private DefendWithDirectionMixin config;

        public AbilityDefendWithDirectionMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (DefendWithDirectionMixin) config;
        }

        protected bool CheckAngle(EvtBeingHit evt)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
            bool flag = Vector3.Angle(base.actor.entity.transform.forward, actor.entity.transform.position - base.actor.entity.transform.position) < this.config.DefendAngle;
            if (this.config.ReverseAngle)
            {
                flag = !flag;
            }
            return flag;
        }

        protected bool CheckSkillID(EvtBeingHit evt)
        {
            if (evt.attackData.isAnimEventAttack)
            {
                if (evt.attackData.rejected)
                {
                    return false;
                }
                if (base.actor.abilityState.ContainsState(AbilityState.Invincible) || base.actor.abilityState.ContainsState(AbilityState.Undamagable))
                {
                    return false;
                }
                if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.DefendPredicates, base.instancedAbility, base.instancedModifier, base.actor, evt))
                {
                    return false;
                }
                if (this.config.AlwaysDefend)
                {
                    return true;
                }
                string currentSkillID = base.actor.entity.CurrentSkillID;
                if (!string.IsNullOrEmpty(currentSkillID))
                {
                    bool flag = false;
                    for (int i = 0; i < this.config.DefendSkillIDs.Length; i++)
                    {
                        if (this.config.DefendSkillIDs[i] == currentSkillID)
                        {
                            flag = true;
                            break;
                        }
                    }
                    float num2 = base.entity.GetCurrentNormalizedTime() % 1f;
                    if ((flag && (num2 > this.config.DefendNormalizedTimeStart)) && (num2 < this.config.DefendNormalizedTimeStop))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Core()
        {
        }

        protected void DefendFailure(EvtBeingHit evt)
        {
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendFailActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
        }

        protected void DefendSuccess(EvtBeingHit evt)
        {
            if (this.config.DefendDamageReduce >= 1f)
            {
                if (!this.config.DefendElemental && (evt.attackData.GetElementalDamage() > 0f))
                {
                    evt.attackData.damage = 0f;
                    evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Mute;
                }
                else
                {
                    evt.attackData.Reject(AttackResult.RejectType.RejectButShowAttackEffect);
                }
            }
            else
            {
                evt.attackData.hitEffect = this.config.DefendSuccessHitEffect;
                evt.attackData.damage *= 1f - this.config.DefendDamageReduce;
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
        }

        protected virtual bool OnBeingHit(EvtBeingHit evt)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
            if ((actor == null) || (actor.entity == null))
            {
                return false;
            }
            if (this.CheckSkillID(evt) && (evt.attackData.attackerAniDamageRatio < this.config.BreakDefendAniDamageRatio))
            {
                if (this.CheckAngle(evt))
                {
                    this.DefendSuccess(evt);
                }
                else
                {
                    this.DefendFailure(evt);
                }
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }
    }
}

