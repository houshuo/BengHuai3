namespace MoleMole.Config
{
    using System;
    using System.Collections.Generic;

    public class ConfigDebuffResistance
    {
        public float DurationRatio;
        public List<AbilityState> ImmuneStates = new List<AbilityState>();
        public float ResistanceRatio;
    }
}

