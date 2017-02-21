namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarTiedMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat UntieSteerAmount;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarTiedMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.UntieSteerAmount != null)
            {
                HashUtils.ContentHashOnto(this.UntieSteerAmount.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.UntieSteerAmount.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.UntieSteerAmount.dynamicKey, ref lastHash);
            }
        }
    }
}

