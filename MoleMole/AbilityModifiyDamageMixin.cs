namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AbilityModifiyDamageMixin : BaseAbilityMixin
    {
        private ModifyDamageMixin config;

        public AbilityModifiyDamageMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ModifyDamageMixin) config;
        }

        private void CheckConfig()
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < this.config.Predicates.Length; i++)
            {
                ConfigAbilityPredicate predicate = this.config.Predicates[i];
                if (predicate is ByAttackAnimEventID)
                {
                    flag = true;
                }
                if (predicate is ByAttackInComboCount)
                {
                    flag2 = true;
                }
            }
            if (!flag && !flag2)
            {
                if (base.instancedAbility.Evaluate(this.config.AddedDamageValue) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.AddedDamagePercentage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.NormalDamage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.FireDamage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.ThunderDamage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.IceDamage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.AllienDamage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.NormalDamagePercentage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.FireDamagePercentage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.ThunderDamagePercentage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.IceDamagePercentage) > 0f)
                {
                    flag3 = true;
                }
                if (base.instancedAbility.Evaluate(this.config.AllienDamagePercentage) > 0f)
                {
                    flag3 = true;
                }
            }
        }

        protected virtual void ModifyDamage(EvtHittingOther evt, float multiple = 1)
        {
            evt.attackData.attackerCritChance += base.instancedAbility.Evaluate(this.config.CritChanceDelta) * multiple;
            evt.attackData.attackerCritDamageRatio += base.instancedAbility.Evaluate(this.config.CritDamageRatioDelta) * multiple;
            evt.attackData.attackerAniDamageRatio += base.instancedAbility.Evaluate(this.config.AnimDamageRatioDelta) * multiple;
            evt.attackData.addedAttackRatio += base.instancedAbility.Evaluate(this.config.AddedAttackRatio) * multiple;
            evt.attackData.addedDamageRatio += base.instancedAbility.Evaluate(this.config.AddedDamageRatio) * multiple;
            evt.attackData.attackerAddedAttackValue += base.instancedAbility.Evaluate(this.config.AddedDamageValue) * multiple;
            evt.attackData.attackerAttackPercentage += base.instancedAbility.Evaluate(this.config.AddedDamagePercentage) * multiple;
            evt.attackData.attackerNormalDamage += base.instancedAbility.Evaluate(this.config.NormalDamage) * multiple;
            evt.attackData.attackerFireDamage += base.instancedAbility.Evaluate(this.config.FireDamage) * multiple;
            evt.attackData.attackerThunderDamage += base.instancedAbility.Evaluate(this.config.ThunderDamage) * multiple;
            evt.attackData.attackerIceDamage += base.instancedAbility.Evaluate(this.config.IceDamage) * multiple;
            evt.attackData.attackerAlienDamage += base.instancedAbility.Evaluate(this.config.AllienDamage) * multiple;
            evt.attackData.attackerNormalDamagePercentage += base.instancedAbility.Evaluate(this.config.NormalDamagePercentage) * multiple;
            evt.attackData.attackerFireDamagePercentage += base.instancedAbility.Evaluate(this.config.FireDamagePercentage) * multiple;
            evt.attackData.attackerThunderDamagePercentage += base.instancedAbility.Evaluate(this.config.ThunderDamagePercentage) * multiple;
            evt.attackData.attackerIceDamagePercentage += base.instancedAbility.Evaluate(this.config.IceDamagePercentage) * multiple;
            evt.attackData.attackerAlienDamagePercentage += base.instancedAbility.Evaluate(this.config.AllienDamagePercentage) * multiple;
            evt.attackData.attackerAddedAllDamageReduceRatio += base.instancedAbility.Evaluate(this.config.AllDamageReduceRatio) * multiple;
        }

        private bool OnHittingOther(EvtHittingOther evt)
        {
            if (!this.config.IncludeNonAnimEventAttacks && !evt.attackData.isAnimEventAttack)
            {
                return false;
            }
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt))
            {
                return false;
            }
            if (UnityEngine.Random.value >= base.instancedAbility.Evaluate(this.config.ModifyChance))
            {
                return false;
            }
            this.ModifyDamage(evt, 1f);
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.Actions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
            evt.attackData.attackerAniDamageRatio += base.instancedAbility.Evaluate(this.config.AniDamageRatio);
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.BreakActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtHittingOther) && this.OnHittingOther((EvtHittingOther) evt));
        }
    }
}

