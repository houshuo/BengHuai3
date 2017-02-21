namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    public class ConfigMisc
    {
        public List<string> ActivityStatusImgPath;
        public ConfigAntiCheat AntiCheat;
        public ConfigAntiEmulator AntiEmulator;
        public List<string> AvatarAttributeBGSpriteList;
        public List<string> AvatarAttributeColorList;
        public List<int> AvatarClassDoNotShow;
        public List<string> AvatarClassSkillIconPath;
        public List<string> AvatarStarIcons;
        public List<string> AvatarStarName;
        public ConfigBasic BasicConfig;
        public ConfigChat ChatConfig;
        public bool CollectUIStatistics;
        public ConfigDynamicArguments Color = new ConfigDynamicArguments();
        public List<int> ComboEvaluation;
        public List<string> ComboNumFrameColor;
        public ConfigDynamicArguments CurrencyIconPath = new ConfigDynamicArguments();
        public ConfigTransformInfo DefaultAvatar3DModel;
        public List<string> DropItemBracketColorList;
        public string DumpFileUploadUrl;
        public List<int> EasternerClassIDList;
        public ConfigDynamicArguments EliteAbilityIcon = new ConfigDynamicArguments();
        public ConfigDynamicArguments EliteAbilityText = new ConfigDynamicArguments();
        public bool EnableHashCheck;
        public List<string> EndlessGroupBGColor;
        public List<string> EndlessGroupSelectPrefabPath;
        public List<string> EndlessGroupUnopenPrefabPath;
        public List<string> EndlessGroupUnSelectColor;
        public List<string> EndlessGroupUnselectPrefabPath;
        public List<int> EquipPowerUpBoostRateResult;
        public ConfigDynamicArguments FeatureUnlockLevel = new ConfigDynamicArguments();
        public ConfigDynamicArguments GachaTicketIconPath = new ConfigDynamicArguments();
        public List<string> GachaTimeTextID;
        public List<string> GachaTypeTitleFigures;
        public List<string> IslandAvatarEnhanceClassImage;
        public List<string> IslandVentureConditionText;
        public List<string> ItemRarityBGImgPath;
        public List<string> ItemRarityColorList;
        public List<string> ItemRarityLightImgPath;
        public List<string> MonthTextIDList;
        public List<ConfigPageAvatarShowInfo> PageAvatarShowInfo;
        public ConfigPlotAvatarCameraPosInfo PlotAvatarCameraPosInfo;
        public ConfigPrefabPath PrefabPath;
        public List<string> RarityColor;
        public List<string> StigmataTypeIconPath;
        public ConfigTextMapKey TextMapKey;
        public List<string> VentureDifficultyDesc;
        public List<string> WeatherBgPath;
    }
}

