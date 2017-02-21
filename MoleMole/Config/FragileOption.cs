namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class FragileOption : ConfigAbilityStateOption
    {
        public DynamicFloat AniDefenceRatio = DynamicFloat.ZERO;
        public DynamicFloat DamageTakeRatio = DynamicFloat.ZERO;
        public DynamicFloat DefenceRatio = DynamicFloat.ZERO;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            modifier.Properties.Add("Actor_DefenceRatio", this.DefenceRatio);
            modifier.Properties.Add("Actor_DamageTakeRatio", this.DamageTakeRatio);
            modifier.Properties.Add("Actor_AniDefenceDelta", this.AniDefenceRatio);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.Fragile;
        }
    }
}

