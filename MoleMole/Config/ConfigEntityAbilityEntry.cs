namespace MoleMole.Config
{
    using System;

    public class ConfigEntityAbilityEntry
    {
        public string AbilityName;
        public string AbilityOverride = "Default";
        public static ConfigEntityAbilityEntry[] EMPTY = new ConfigEntityAbilityEntry[0];
    }
}

