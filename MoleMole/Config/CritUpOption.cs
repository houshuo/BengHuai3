namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class CritUpOption : ConfigAbilityStateOption
    {
        public DynamicFloat CritChanceDelta;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            modifier.Properties.Add("Actor_CriticalChanceDelta", this.CritChanceDelta);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.CritUp;
        }
    }
}

