namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachRebindAttachPointMixin : ConfigAbilityMixin, IHashable
    {
        public string OriginName;
        public string OtherName;
        public string PointName;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAttachRebindAttachPointMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.PointName, ref lastHash);
            HashUtils.ContentHashOnto(this.OriginName, ref lastHash);
            HashUtils.ContentHashOnto(this.OtherName, ref lastHash);
        }
    }
}

