namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachModifierToLevelBuffMixin : ConfigAbilityMixin, IHashable
    {
        public LevelBuffType LevelBuff;
        public string OffModifierName;
        public string OnModifierName;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAttachModifierToLevelBuffMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.LevelBuff, ref lastHash);
            HashUtils.ContentHashOnto(this.OnModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.OffModifierName, ref lastHash);
        }
    }
}

