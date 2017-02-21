namespace MoleMole.Config
{
    using System;

    public class ConfigAvatarCommonArguments : ConfigEntityCommonArguments
    {
        public float AttackSPRecoverRatio = 3f;
        public float GoodsAttractRadius;
        public string[] MaskedSkillButtons = Miscs.EMPTY_STRINGS;
        public float QTESwitchInCDRatio = 2f;
        public float SwitchInCD = 4f;
    }
}

