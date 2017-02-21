namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityModifyDamageTakenByDamageValueMixin : AbilityModifyDamageTakenMixin
    {
        private ModifyDamageTakenByDamageValueMixin config;

        public AbilityModifyDamageTakenByDamageValueMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ModifyDamageTakenByDamageValueMixin) config;
        }

        private float GetReplaceDamage(float damage)
        {
            ModifyDamageTakenByDamageValueMixin.LogicType compareType = this.config.CompareType;
            if (compareType != ModifyDamageTakenByDamageValueMixin.LogicType.MoreThan)
            {
                if (compareType != ModifyDamageTakenByDamageValueMixin.LogicType.LessThanOrEqual)
                {
                    return damage;
                }
            }
            else
            {
                if (damage > base.instancedAbility.Evaluate(this.config.ByDamageValue))
                {
                    damage = base.instancedAbility.Evaluate(this.config.ReplaceDamageValue);
                }
                return damage;
            }
            if ((damage <= base.instancedAbility.Evaluate(this.config.ByDamageValue)) && (damage > base.instancedAbility.Evaluate(this.config.ReplaceDamageValue)))
            {
                damage = base.instancedAbility.Evaluate(this.config.ReplaceDamageValue);
            }
            return damage;
        }

        protected override bool OnPostBeingHit(EvtBeingHit evt)
        {
            base.OnPostBeingHit(evt);
            if (this.GetReplaceDamage(evt.attackData.damage) != evt.attackData.damage)
            {
                evt.attackData.damage = this.GetReplaceDamage(evt.attackData.damage);
                if (this.config.UseReplaceAniDamageRatio)
                {
                    evt.attackData.attackerAniDamageRatio = this.config.ReplaceAniDamageRatio;
                }
            }
            evt.attackData.fireDamage = this.GetReplaceDamage(evt.attackData.fireDamage);
            evt.attackData.thunderDamage = this.GetReplaceDamage(evt.attackData.thunderDamage);
            evt.attackData.alienDamage = this.GetReplaceDamage(evt.attackData.alienDamage);
            evt.attackData.plainDamage = this.GetReplaceDamage(evt.attackData.plainDamage);
            return true;
        }
    }
}

