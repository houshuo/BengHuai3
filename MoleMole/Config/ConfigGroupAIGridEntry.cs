namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Collections.Generic;

    public class ConfigGroupAIGridEntry : IOnLoaded
    {
        public Dictionary<string, List<ConfigLeaderToMinionAction>> LeaderActions = new Dictionary<string, List<ConfigLeaderToMinionAction>>();
        public ConfigOverrideList Minions = ConfigOverrideList.EMPTY;
        public string Name;

        public void OnLoaded()
        {
        }
    }
}

