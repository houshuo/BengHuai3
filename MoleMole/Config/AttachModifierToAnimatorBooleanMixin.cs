namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachModifierToAnimatorBooleanMixin : ConfigAbilityMixin, IHashable
    {
        public string AnimatorBoolean;
        public bool IsInvert;
        public string ModifierName;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAttachModifierToAnimatorBooleanMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.AnimatorBoolean, ref lastHash);
            HashUtils.ContentHashOnto(this.ModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.IsInvert, ref lastHash);
        }
    }
}

