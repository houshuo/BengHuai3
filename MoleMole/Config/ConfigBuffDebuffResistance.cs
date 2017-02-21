namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Collections.Generic;

    public class ConfigBuffDebuffResistance
    {
        public float DurationRatio;
        public List<AbilityState> ResistanceBuffDebuffs = new List<AbilityState>();
        public float ResistanceRatio;

        public ConfigBuffDebuffResistance(AbilityState[] abilityStates, float resistRatio, float durationRatio)
        {
            for (int i = 0; i < abilityStates.Length; i++)
            {
                this.ResistanceBuffDebuffs.Add(abilityStates[i]);
            }
            this.ResistanceRatio = resistRatio;
            this.DurationRatio = durationRatio;
        }
    }
}

