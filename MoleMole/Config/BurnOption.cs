namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class BurnOption : ConfigAbilityStateOption
    {
        public DynamicFloat BurnCD;
        public DynamicFloat BurnDamage = DynamicFloat.ZERO;
        public DynamicFloat DamagePercentage = DynamicFloat.ZERO;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            modifier.ThinkInterval = this.BurnCD;
            DamageByAttackValue element = new DamageByAttackValue {
                Target = AbilityTargetting.Self,
                FireDamagePercentage = this.DamagePercentage,
                FireDamage = this.BurnDamage
            };
            Miscs.ArrayAppend<ConfigAbilityAction>(ref modifier.OnThinkInterval, element);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.Burn;
        }
    }
}

