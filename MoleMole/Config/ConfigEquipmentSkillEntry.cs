namespace MoleMole.Config
{
    using System;

    public class ConfigEquipmentSkillEntry
    {
        public string AbilityName;
        public string AbilityOverride = "Default";
        public int EquipmentSkillID;
        public bool IsActiveSkill;
        public bool IsInstantTrigger;
        public int MaxChargesCount;
        public ParamMethod ParamMethod1;
        public ParamMethod ParamMethod2;
        public ParamMethod ParamMethod3;
        public string ParamSpecial1;
        public string ParamSpecial2;
        public string ParamSpecial3;
        public int SkillCD;
        public float SPCost;
        public float SPNeed;
    }
}

