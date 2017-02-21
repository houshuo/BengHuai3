namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class PowerUpOption : ConfigAbilityStateOption
    {
        public DynamicFloat AniDamageRatio = DynamicFloat.ZERO;
        public DynamicFloat AttackRatio;

        public override void ChangeModifierConfig(ConfigAbilityModifier modifier)
        {
            modifier.Properties.Add("Actor_AddedAttackRatio", this.AttackRatio);
            modifier.Properties.Add("Actor_AniDamageDelta", this.AniDamageRatio);
        }

        public override AbilityState GetMatchingAbilityState()
        {
            return AbilityState.PowerUp;
        }
    }
}

