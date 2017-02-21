namespace MoleMole.Config
{
    using FullInspector;
    using System;

    public class StageEntry
    {
        public readonly string EnvPrefabPath;
        public string HideNodeNames;
        public string HideNodePrefabPaths;
        public readonly string LocationPointName;
        public readonly string PerpStagePrefabPath;
        public string ShowNodeNames;
        public string ShowNodePrefabPaths;
        private const string STAGE_PREFAB_PATH = "Stage/";
        [InspectorNullable]
        public ConfigStageEffectSetting StageEffectSetting;
        public readonly string TypeName;

        public string GetEnvPrefabPath()
        {
            return ("Stage/" + this.EnvPrefabPath);
        }

        public string GetPerpStagePrefabPath()
        {
            return ("Stage/" + this.PerpStagePrefabPath);
        }
    }
}

