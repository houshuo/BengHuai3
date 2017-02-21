namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ModifyDamageTakenByDamageValueMixin : ModifyDamageTakenMixin
    {
        public DynamicFloat ByDamageValue;
        public LogicType CompareType;
        public float ReplaceAniDamageRatio;
        public DynamicFloat ReplaceDamageValue;
        public bool UseReplaceAniDamageRatio;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityModifyDamageTakenByDamageValueMixin(instancedAbility, instancedModifier, this);
        }

        public enum LogicType
        {
            MoreThan,
            LessThanOrEqual
        }
    }
}

