namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ModifyDamageTakenWithMultiMixin : ModifyDamageTakenMixin
    {
        public float BaseMultiple;
        public DynamicFloat MaxMultiple = DynamicFloat.ZERO;
        public DamageMultipleType MultipleType;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityModifyDamageTakenWithMultiMixin(instancedAbility, instancedModifier, this);
        }

        public enum DamageMultipleType
        {
            ByTargetDistance
        }
    }
}

