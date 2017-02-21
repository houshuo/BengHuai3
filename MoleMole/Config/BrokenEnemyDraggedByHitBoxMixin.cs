namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class BrokenEnemyDraggedByHitBoxMixin : ConfigAbilityMixin, IHashable
    {
        public string ColliderEntryName;
        public DynamicFloat PullVelocity;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityBrokenEnemyDraggedByHitBoxMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.ColliderEntryName, ref lastHash);
            if (this.PullVelocity != null)
            {
                HashUtils.ContentHashOnto(this.PullVelocity.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PullVelocity.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PullVelocity.dynamicKey, ref lastHash);
            }
        }
    }
}

