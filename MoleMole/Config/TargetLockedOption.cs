namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class TargetLockedOption : ConfigAbilityStateOption
    {
        public DynamicFloat TakeExtraDamageRatio;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            ModifyDamageByAttackeeMixin element = new ModifyDamageByAttackeeMixin {
                AddedDamageTakeRatio = this.TakeExtraDamageRatio,
                Predicates = new ConfigAbilityPredicate[0]
            };
            Miscs.ArrayAppend<ConfigAbilityMixin>(ref modifier.ModifierMixins, element);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.TargetLocked;
        }
    }
}

