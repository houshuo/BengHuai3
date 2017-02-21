namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class PoisonOption : ConfigAbilityStateOption
    {
        public DynamicFloat DamagePercentage = DynamicFloat.ZERO;
        public DynamicFloat PoisonCD;
        public DynamicFloat PoisonDamage = DynamicFloat.ZERO;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            modifier.ThinkInterval = this.PoisonCD;
            DamageByAttackValue element = new DamageByAttackValue {
                Target = AbilityTargetting.Self,
                DamagePercentage = this.DamagePercentage,
                AddedDamageValue = this.PoisonDamage
            };
            Miscs.ArrayAppend<ConfigAbilityAction>(ref modifier.OnThinkInterval, element);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.Poisoned;
        }
    }
}

