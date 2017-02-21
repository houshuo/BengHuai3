namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ShieldedOption : ConfigAbilityStateOption
    {
        public DynamicFloat AniDefenceRatio = DynamicFloat.ZERO;
        public DynamicFloat DefenceRatio;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            modifier.Properties.Add("Actor_DefenceRatio", this.DefenceRatio);
            modifier.Properties.Add("Actor_AniDefenceDelta", this.AniDefenceRatio);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.Shielded;
        }
    }
}

