namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ConfigStageEffectSetting
    {
        public ColorOverrideEntry[] AvatarColorOverrides = ColorOverrideEntry.EMPTY;
        public static ConfigStageEffectSetting EMPTY = new ConfigStageEffectSetting();
        public string[] LocalAvatarEffectPredicates = Miscs.EMPTY_STRINGS;
    }
}

