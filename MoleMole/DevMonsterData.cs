namespace MoleMole
{
    using System;

    [Serializable]
    public class DevMonsterData
    {
        public string[] abilities;
        public bool isElite;
        public bool isStationary;
        public int level;
        public string monsterName;
        public string typeName;
        public uint uniqueMonsterID;

        public DevMonsterData Clone()
        {
            return new DevMonsterData { monsterName = this.monsterName, typeName = this.typeName, isStationary = this.isStationary, isElite = this.isElite, abilities = this.abilities };
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", this.monsterName, this.typeName);
        }
    }
}

