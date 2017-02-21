namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public static class DevLevelConfigData
    {
        public static LevelManager _levelManager;
        public static List<DevAvatarData> avatarDevDatas = new List<DevAvatarData>();
        public static List<string> avatarTypeNames = new List<string>();
        public static bool configFromScene = false;
        public static bool isBenchmark = false;
        public static int LEVEL_DIFFICULTY = 0;
        public static LevelActor.Mode LEVEL_MODE = LevelActor.Mode.Single;
        public static string LEVEL_PATH = null;
        public static List<DevMonsterData> monsterDevDatas = new List<DevMonsterData>();
        public static List<int> monsterInstanceIds = new List<int>();
        public static bool pariticleMode = true;
        public static DevStageData stageDevData;
    }
}

