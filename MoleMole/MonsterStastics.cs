namespace MoleMole
{
    using System;

    public class MonsterStastics
    {
        public SafeFloat aliveTime = 0f;
        public SafeInt32 breakAvatarTimes = 0;
        public SafeFloat damage = 0f;
        public SafeFloat dps = 0f;
        public SafeInt32 hitAvatarTimes = 0;
        public bool isAlive = true;
        public MonsterKey key;
        public SafeInt32 monsterCount = 0;

        public MonsterStastics(string monsterName, string configType, int level)
        {
            MonsterKey key = new MonsterKey {
                monsterName = monsterName,
                configType = configType,
                level = level
            };
            this.key = key;
            this.dps = 0f;
        }
    }
}

