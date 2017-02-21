namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityModifyAnimEventAttackMixin : BaseAbilityMixin
    {
        private ModifyAnimEventAttackMixin config;

        public AbilityModifyAnimEventAttackMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ModifyAnimEventAttackMixin) config;
        }

        private bool OnHittingOtherResolve(EvtHittingOther evt)
        {
            bool flag = false;
            if (this.config.ModifyAllAnimEvents)
            {
                flag = true;
            }
            else
            {
                flag = Miscs.ArrayContains<string>(this.config.AnimEventIDs, evt.animEventID);
            }
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt))
            {
                return false;
            }
            if (flag)
            {
                evt.attackData.attackerCritChance += base.instancedAbility.Evaluate(this.config.CritChanceDelta);
                evt.attackData.attackerCritDamageRatio += base.instancedAbility.Evaluate(this.config.CritDamageRatioDelta);
                evt.attackData.attackerAniDamageRatio += base.instancedAbility.Evaluate(this.config.AnimDamageRatioDelta);
                evt.attackData.attackerAttackPercentage += base.instancedAbility.Evaluate(this.config.DamagePercentageDelta);
                evt.attackData.attackerShieldDamageRatio += base.instancedAbility.Evaluate(this.config.ShieldDamageDelta);
                evt.attackData.attackerAttackValue += base.instancedAbility.Evaluate(this.config.AttackValueDelta);
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

