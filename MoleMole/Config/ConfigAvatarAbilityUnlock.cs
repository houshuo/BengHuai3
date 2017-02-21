namespace MoleMole.Config
{
    using System;

    public class ConfigAvatarAbilityUnlock
    {
        public string AbilityName;
        public string AbilityOverride = "Default";
        public string AbilityReplaceID;
        public static ConfigAvatarAbilityUnlock[] EMTPY = new ConfigAvatarAbilityUnlock[0];
        public bool IsUnlockBySkill;
        public ParamMethod ParamMethod1;
        public ParamMethod ParamMethod2;
        public ParamMethod ParamMethod3;
        public string ParamSpecial1;
        public string ParamSpecial2;
        public string ParamSpecial3;
        public int UnlockBySkillID;
        public int UnlockBySubSkillID;
    }
}

