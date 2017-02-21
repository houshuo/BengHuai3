namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class RangeAttackProtectShieldMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat DamageReduceRatio;
        public ConfigAbilityAction[] OnRangeAttackProtectShieldSuccessActions;
        public DynamicFloat ProtectRange;

        public RangeAttackProtectShieldMixin()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 3f
            };
            this.ProtectRange = num;
            this.DamageReduceRatio = DynamicFloat.ZERO;
            this.OnRangeAttackProtectShieldSuccessActions = ConfigAbilityAction.EMPTY;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityRangeAttackProtectShieldMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.OnRangeAttackProtectShieldSuccessActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ProtectRange != null)
            {
                HashUtils.ContentHashOnto(this.ProtectRange.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ProtectRange.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ProtectRange.dynamicKey, ref lastHash);
            }
            if (this.DamageReduceRatio != null)
            {
                HashUtils.ContentHashOnto(this.DamageReduceRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageReduceRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageReduceRatio.dynamicKey, ref lastHash);
            }
            if (this.OnRangeAttackProtectShieldSuccessActions != null)
            {
                foreach (ConfigAbilityAction action in this.OnRangeAttackProtectShieldSuccessActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
        }
    }
}

