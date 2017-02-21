namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityDistanceBeyondMixin : BaseAbilityMixin
    {
        private DistanceBeyondMixin config;

        public AbilityDistanceBeyondMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (DistanceBeyondMixin) config;
        }

        private bool OnHittingOtherResolve(EvtHittingOther evt)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID);
            if ((actor == null) || (actor.entity == null))
            {
                return false;
            }
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt))
            {
                return false;
            }
            Vector3 vector = base.actor.entity.transform.position - actor.entity.transform.position;
            bool flag = vector.magnitude > base.instancedAbility.Evaluate(this.config.HitDistanceBeyond);
            if (this.config.Reverse)
            {
                flag = !flag;
            }
            if (flag)
            {
                evt.attackData.attackerCritChance += base.instancedAbility.Evaluate(this.config.CriticalChanceRatioUp);
                evt.attackData.attackerCritDamageRatio += base.instancedAbility.Evaluate(this.config.CriticalDamageRatioUp);
                evt.attackData.attackerAttackPercentage += base.instancedAbility.Evaluate(this.config.DamagePercentageUp);
                evt.attackData.attackerAniDamageRatio += base.instancedAbility.Evaluate(this.config.AniDamageRatioUp);
                evt.attackData.attackerAttackValue *= 1f + base.instancedAbility.Evaluate(this.config.AttackRatio);
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.Actions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtHittingOther) && this.OnHittingOtherResolve((EvtHittingOther) evt));
        }
    }
}

