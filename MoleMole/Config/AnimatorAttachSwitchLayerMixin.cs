namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AnimatorAttachSwitchLayerMixin : ConfigAbilityMixin, IHashable
    {
        public float Duration = 0.3f;
        public int FromLayer;
        public bool NoEndResume;
        public int ToLayer;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAnimatorAttachSwitchLayerMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.FromLayer, ref lastHash);
            HashUtils.ContentHashOnto(this.ToLayer, ref lastHash);
            HashUtils.ContentHashOnto(this.Duration, ref lastHash);
            HashUtils.ContentHashOnto(this.NoEndResume, ref lastHash);
        }
    }
}

