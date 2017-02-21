namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class PullTargetMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat PullRadius;
        public DynamicFloat PullVelocity;
        public DynamicFloat StopDistance = DynamicFloat.ONE;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityPullTargetMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.PullRadius != null)
            {
                HashUtils.ContentHashOnto(this.PullRadius.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PullRadius.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PullRadius.dynamicKey, ref lastHash);
            }
            if (this.PullVelocity != null)
            {
                HashUtils.ContentHashOnto(this.PullVelocity.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PullVelocity.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PullVelocity.dynamicKey, ref lastHash);
            }
            if (this.StopDistance != null)
            {
                HashUtils.ContentHashOnto(this.StopDistance.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.StopDistance.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.StopDistance.dynamicKey, ref lastHash);
            }
        }
    }
}

