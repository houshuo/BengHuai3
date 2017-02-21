namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachAdditiveVelocityMixin : ConfigAbilityMixin, IHashable
    {
        public float MoveAngle;
        public float MoveSpeed;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAttachAdditiveVelocityMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.MoveSpeed, ref lastHash);
            HashUtils.ContentHashOnto(this.MoveAngle, ref lastHash);
        }
    }
}

