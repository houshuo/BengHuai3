namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class BleedOption : ConfigAbilityStateOption
    {
        public DynamicFloat BleedCD;
        public DynamicFloat BleedDamage = DynamicFloat.ZERO;
        public DynamicFloat DamagePercentage = DynamicFloat.ZERO;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            modifier.ThinkInterval = this.BleedCD;
            DamageByAttackValue element = new DamageByAttackValue {
                Target = AbilityTargetting.Self,
                DamagePercentage = this.DamagePercentage,
                AddedDamageValue = this.BleedDamage
            };
            Miscs.ArrayAppend<ConfigAbilityAction>(ref modifier.OnThinkInterval, element);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.Bleed;
        }
    }
}

