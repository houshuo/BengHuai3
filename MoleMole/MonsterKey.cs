namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MonsterKey
    {
        public string monsterName;
        public string configType;
        public int level;
    }
}

