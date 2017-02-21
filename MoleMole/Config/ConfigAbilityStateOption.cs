namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public abstract class ConfigAbilityStateOption
    {
        protected ConfigAbilityStateOption()
        {
        }

        public abstract void ChangeModifierConfig(ConfigAbilityModifier modifier);
        public abstract AbilityState GetMatchingAbilityState();
    }
}

