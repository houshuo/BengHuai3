namespace MoleMole.Config
{
    using System;
    using System.Collections.Generic;

    public class ConfigGroupAIMinionOld
    {
        public string AIName;
        public ConfigGroupAIMinionParamOld[] AIParams = new ConfigGroupAIMinionParamOld[0];
        public AttackType DefaultAttackType;
        public MoveType DefaultMoveType;
        public string MonsterName;
        public Dictionary<string, ConfigGroupAIMinionParamOld[]> TriggerAtcions = new Dictionary<string, ConfigGroupAIMinionParamOld[]>();

        public enum AttackType
        {
            Trigger,
            Free
        }

        public enum MoveType
        {
            Sync,
            Follow,
            Free
        }
    }
}

