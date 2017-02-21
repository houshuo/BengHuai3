namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class StealHPMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat HPStealRatio = DynamicFloat.ZERO;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityStealMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.HPStealRatio != null)
            {
                HashUtils.ContentHashOnto(this.HPStealRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HPStealRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HPStealRatio.dynamicKey, ref lastHash);
            }
        }
    }
}

