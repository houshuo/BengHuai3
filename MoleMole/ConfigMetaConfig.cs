namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;

    public class ConfigMetaConfig : BaseScriptableObject
    {
        public string[] abilityRegistryPathes;
        [NonSerialized]
        public string allLevelsClearedAtmosphereRegistryPath = "Rendering/MainMenuAtmosphereConfig/Data/AllLevelsClearedAtmosphereRegistry";
        public string[] animatorEventPatternPathes;
        [NonSerialized]
        public string atmosphereRegistryPath = "Rendering/MainMenuAtmosphereConfig/Data/AtmosphereRegistry";
        public string[] auxEntryPathes;
        public string[] cameraCurvePatternPathes;
        public string[] dynamicObjectRegistryPathes;
        public string[] effectPatternPathes;
        public string[] equipmentSkillRegistryPathes;
        public string[] galTouchBuffRegistryPathes;
        [NonSerialized]
        public string galTouchDataPath = "Data/GalTouch/GalTouchData";
        public string[] graphicsSettingRegistryPathes;
        [NonSerialized]
        public string graphicsVolatileSettingRegistryPath = "Data/_BothLocalAndAssetBundle/GraphicsSettingConfig/VolatileSetting";
        public string[] groupAIGridPathes;
        [NonSerialized]
        public string inLevelMiscData = "Data/InLevelMiscData";
        public string[] propObjectRegistryPathes;
        public string[] renderEntryPathes;
        public ConfigNavMeshScenePath[] scenePaths;
        public string[] sharedAnimEventGroupPathes;
        public string[] stageEntryPathes;
        public string[] touchPatternPathes;
        public string[] weaponRegistryPathes;
        public string[] weatherEntryPathes;
    }
}

