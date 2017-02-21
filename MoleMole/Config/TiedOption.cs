namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class TiedOption : ConfigAbilityStateOption
    {
        public DynamicFloat UntieSteerAmount;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            AvatarTiedMixin element = new AvatarTiedMixin {
                UntieSteerAmount = this.UntieSteerAmount
            };
            Miscs.ArrayAppend<ConfigAbilityMixin>(ref modifier.ModifierMixins, element);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.Tied;
        }
    }
}

