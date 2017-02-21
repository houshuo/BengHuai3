namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachModifierToPartialSkillIDMixin : ConfigAbilityMixin, IHashable
    {
        public string ModifierName;
        public float NormalizedTimeStart;
        public float NormalizedTimeStop;
        public string SkillID;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAttachModifierToPartialSkillIDMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.SkillID, ref lastHash);
            HashUtils.ContentHashOnto(this.NormalizedTimeStart, ref lastHash);
            HashUtils.ContentHashOnto(this.NormalizedTimeStop, ref lastHash);
            HashUtils.ContentHashOnto(this.ModifierName, ref lastHash);
        }
    }
}

