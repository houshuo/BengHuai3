namespace MoleMole.Config
{
    using System;

    public class ConfigGroupAIMinionParamOld
    {
        public ConfigGroupAIMinionOld.AttackType AttackTypeValue;
        public bool BoolValue;
        public float FloatValue;
        public bool Interruption;
        public int IntValue;
        public ConfigGroupAIMinionOld.MoveType MoveTypeValue;
        public string Name;
        public bool TriggerAttack;
        public float TriggerAttackDelay;
        public ParamType Type = ParamType.None;

        public enum ParamType
        {
            Float,
            Int,
            Bool,
            AttackType,
            MoveType,
            None
        }
    }
}

