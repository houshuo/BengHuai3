namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ModifyDamageWithMultiMixin : ModifyDamageMixin
    {
        public float BaseMultiple;
        public bool ClearAllSP = true;
        public DynamicFloat MaxMultiple = DynamicFloat.ZERO;
        public DamageMultipleType MultipleType;
        public AbilityState TargetAbilityState;
        public MixinTargetting Targetting;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityModifiyDamageWithMultiMixin(instancedAbility, instancedModifier, this);
        }

        public enum DamageMultipleType
        {
            BySelfCurrentSPAmount,
            BySelfMaxSPAmount,
            ByTargetAbilityState,
            ByTargetDistance,
            ByLevelCurrentCombo
        }
    }
}

