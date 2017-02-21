namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachModifierToSkillIDMixin : ConfigAbilityMixin, IHashable
    {
        public bool Inverse;
        public string ModifierName;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;
        public string[] SkillIDs;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAttachModifierToSkillIDMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.SkillIDs != null)
            {
                foreach (string str in this.SkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.ModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.Inverse, ref lastHash);
            if (this.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in this.Predicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
        }
    }
}

